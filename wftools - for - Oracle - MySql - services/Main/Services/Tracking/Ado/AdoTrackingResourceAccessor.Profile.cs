using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text;
using System.Workflow.Runtime.Tracking;
using WFTools.Services.Common.Ado;
using WFTools.Services.Tracking.Entity;
using WFTools.Utilities;

namespace WFTools.Services.Tracking.Ado
{
    //
    // Partial implementation of AdoTrackingResourceAccessor containing 
    // profile manipulation and retrieval methods
    //
    public partial class AdoTrackingResourceAccessor
    {
        ///<summary>
        /// Retrieves the tracking profile for the specified workflow type 
        /// if one is available.
        ///</summary>
        ///<returns>
        ///true if a <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> for the specified workflow <see cref="T:System.Type"></see> is available; otherwise, false. If true, the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> is returned in profile.
        ///</returns>
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow for which to get the tracking profile.</param>
        ///<param name="profile">When this method returns, contains the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> to load. This parameter is passed un-initialized.</param>
        public Boolean TryGetTrackingProfile(Type workflowType, out TrackingProfile profile)
        {
            if (workflowType == null)
                throw new ArgumentNullException("workflowType");

            profile = getProfile(workflowType, emptyVersion, true);

            return (profile != null);
        }

        ///<summary>
        /// Returns the tracking profile, qualified by version, for the 
        /// specified workflow <see cref="T:System.Type"></see>. 
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see>.
        ///</returns>
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow.</param>
        ///<param name="profileVersion">The <see cref="T:System.Version"></see> of the tracking profile.</param>
        public TrackingProfile GetTrackingProfile(Type workflowType, Version profileVersion)
        {
            if (workflowType == null)
                throw new ArgumentNullException("workflowType");

            return getProfile(workflowType, profileVersion, false);
        }

        ///<summary>
        /// Returns the tracking profile for the specified workflow instance.
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see>.
        ///</returns>
        ///<param name="instanceId">The <see cref="T:System.Guid"></see> of the workflow instance.</param>
        public TrackingProfile GetTrackingProfile(Guid instanceId)
        {
            return getProfile(instanceId);
        }

        ///<summary>
        /// Returns the latest default tracking profile.
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see>.
        ///</returns>
        public TrackingProfile GetDefaultTrackingProfile()
        {
            TrackingProfile trackingProfile = null;
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.GetCurrentDefaultTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetCurrentDefaultTrackingProfile,
                    TrackingParameterName.TrackingProfile),
                    AdoDbType.Cursor, ParameterDirection.Output);

                using (DbDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        trackingProfile = buildProfileFromXml(_valueReader.GetString(dataReader, 1));
                }
            }

            return trackingProfile;
        }

        ///<summary>
        /// Returns the default tracking profile, qualified by version.
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see>.
        ///</returns>
        ///<param name="profileVersion">The <see cref="T:System.Version"></see> of the tracking profile.</param>
        public TrackingProfile GetDefaultTrackingProfile(Version profileVersion)
        {
            if (profileVersion == null)
                throw new ArgumentNullException("profileVersion");

            TrackingProfile trackingProfile = null;
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.GetCurrentDefaultTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetDefaultTrackingProfile,
                    TrackingParameterName.Version), profileVersion.ToString(),
                    AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetDefaultTrackingProfile,
                    TrackingParameterName.TrackingProfile),
                    AdoDbType.Cursor, ParameterDirection.Output);

                using (DbDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        trackingProfile = buildProfileFromXml(_valueReader.GetString(dataReader, 0));
                }
            }

            return trackingProfile;
        }

        ///<summary>
        /// Retrieves a new tracking profile for the specified workflow instance 
        /// if the tracking profile has changed since it was last loaded.
        ///</summary>
        ///<returns>
        ///true if a new <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> should be loaded; otherwise, false. If true, the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> is returned in profile.
        ///</returns>
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow instance.</param>
        ///<param name="profile">When this method returns, contains the <see cref="T:System.Workflow.Runtime.Tracking.TrackingProfile"></see> to load. This parameter is passed un-initialized.</param>
        ///<param name="instanceId">The <see cref="T:System.Guid"></see> of the workflow instance.</param>
        public Boolean TryReloadTrackingProfile(Type workflowType, Guid instanceId, out TrackingProfile profile)
        {
            if (workflowType == null)
                throw new ArgumentNullException("workflowType");

            profile = getProfile(instanceId);

            return (profile != null);
        }

        /// <summary>
        /// Retrieve a list of tracking profile changes since the last update.
        /// </summary>
        /// <param name="lastCheck">
        /// Indicates the <see cref="DateTime" /> when the changes were last checked and, 
        /// after the method has completed indicates when the check occurred in the
        /// tracking store.
        /// </param>
        /// <returns>
        /// An <see cref="IList{T}" /> containing <see cref="TrackingProfileChange" /> objects.
        /// </returns>
        public IList<TrackingProfileChange> GetTrackingProfileChanges(ref DateTime lastCheck)
        {
            List<TrackingProfileChange> trackingProfileChanges = new List<TrackingProfileChange>();

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.GetTrackingProfileChanges), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetTrackingProfileChanges,
                    TrackingParameterName.LastCheck),
                    lastCheck, AdoDbType.DateTime);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetTrackingProfileChanges,
                    TrackingParameterName.NextCheck),
                    AdoDbType.DateTime, ParameterDirection.Output);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetTrackingProfileChanges,
                    TrackingParameterName.TrackingProfile),
                    AdoDbType.Cursor, ParameterDirection.Output);

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        String typeFullName = _valueReader.GetString(dataReader, 0);
                        String assemblyFullName = _valueReader.GetString(dataReader, 1);
                        Type workflowType = TypeUtilities.GetType(typeFullName, assemblyFullName);
                        if (workflowType != null)
                        {
                            TrackingProfileChange trackingProfileChange = new TrackingProfileChange();
                            trackingProfileChange.WorkflowType = workflowType;
                            trackingProfileChange.TrackingProfile = buildProfileFromXml(
                                _valueReader.GetString(dataReader, 2));

                            trackingProfileChanges.Add(trackingProfileChange);
                        }
                    }
                }

                DateTime? newLastCheck = _valueReader.GetNullableDateTime(
                    dbCommand, _nameResolver.ResolveParameterName(
                        TrackingCommandName.GetTrackingProfileChanges,
                        TrackingParameterName.NextCheck));

                if (newLastCheck != null)
                    lastCheck = newLastCheck.Value;
            }

            return trackingProfileChanges;
        }

        ///<summary>
        /// Returns the latest tracking profile for the specified workflow 
        /// <see cref="T:System.Type" />.
        ///</summary>
        ///<returns>
        ///A <see cref="TrackingProfile"></see>.
        ///</returns>
        ///<param name="workflowType">The <see cref="T:System.Type"></see> of the workflow.</param>
        public TrackingProfile GetTrackingProfile(Type workflowType)
        {
            return getProfile(workflowType, emptyVersion, false);
        }

        /// <summary>
        /// Updates the tracking profile for the specified workflow <see cref="Type" />.
        /// </summary>
        /// <param name="workflowType">The <see cref="Type"></see> of the workflow.</param>
        /// <param name="updatedProfile">The updated <see cref="TrackingProfile" />.</param>
        public void UpdateTrackingProfile(Type workflowType, TrackingProfile updatedProfile)
        {
            if (workflowType == null)
                throw new ArgumentNullException("workflowType");

            if (updatedProfile == null)
                throw new ArgumentNullException("updatedProfile");

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.UpdateTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateTrackingProfile,
                    TrackingParameterName.TypeFullName),
                    workflowType.FullName, AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateTrackingProfile,
                    TrackingParameterName.AssemblyFullName),
                    workflowType.Assembly.FullName, AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateTrackingProfile,
                    TrackingParameterName.Version),
                    updatedProfile.Version.ToString(), AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateTrackingProfile,
                    TrackingParameterName.TrackingProfile),
                    buildXmlFromProfile(updatedProfile), AdoDbType.Text);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates the tracking profile for the specified workflow instance.
        /// </summary>
        /// <param name="instanceId">The <see cref="Guid"></see> of the workflow instance.</param>
        /// <param name="updatedProfile">The updated <see cref="TrackingProfile" />.</param>
        public void UpdateTrackingProfile(Guid instanceId, TrackingProfile updatedProfile)
        {
            if (updatedProfile == null)
                throw new ArgumentNullException("updatedProfile");

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.UpdateInstanceTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateInstanceTrackingProfile,
                    TrackingParameterName.InstanceId), instanceId,
                    AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateInstanceTrackingProfile,
                    TrackingParameterName.Version),
                    updatedProfile.Version.ToString(), AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateInstanceTrackingProfile,
                    TrackingParameterName.TrackingProfile),
                    buildXmlFromProfile(updatedProfile), AdoDbType.Text);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates the default tracking profile.
        /// </summary>
        /// <param name="updatedProfile">The updated default <see cref="TrackingProfile" />.</param>
        public void UpdateDefaultTrackingProfile(TrackingProfile updatedProfile)
        {
            if (updatedProfile == null)
                throw new ArgumentNullException("updatedProfile");

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.UpdateDefaultTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateDefaultTrackingProfile,
                    TrackingParameterName.Version),
                    updatedProfile.Version.ToString(), AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.UpdateDefaultTrackingProfile,
                    TrackingParameterName.TrackingProfile),
                    buildXmlFromProfile(updatedProfile), AdoDbType.Text);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the tracking profile for the specified workflow <see cref="Type" />.
        /// </summary>
        /// <param name="workflowType">The <see cref="Type"></see> of the workflow.</param>
        public void DeleteTrackingProfile(Type workflowType)
        {
            if (workflowType == null)
                throw new ArgumentNullException("workflowType");

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.DeleteTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.DeleteTrackingProfile,
                    TrackingParameterName.TypeFullName), workflowType.FullName,
                    AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.DeleteTrackingProfile,
                    TrackingParameterName.AssemblyFullName),
                    workflowType.Assembly.FullName, AdoDbType.String);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the tracking profile for the workflow instance with the 
        /// specified identifier.
        /// </summary>
        /// <param name="instanceId">The <see cref="Guid"></see> of the workflow instance.</param>
        public void DeleteTrackingProfile(Guid instanceId)
        {
            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.DeleteInstanceTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.DeleteInstanceTrackingProfile,
                    TrackingParameterName.InstanceId), instanceId,
                    AdoDbType.Guid);

                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Helper method used to retrieve a tracking profile from the tracking store.
        /// </summary>
        /// <param name="workflowType">The <see cref="Type" /> of the workflow.</param>
        /// <param name="profileVersion">The <see cref="Version" /> of the tracking profile.</param>
        /// <param name="createDefault">Indicates whether to </param>
        /// <returns>
        /// A <see cref="TrackingProfile" /> for the specified workflow type and version.
        /// </returns>
        private TrackingProfile getProfile(Type workflowType, Version profileVersion, Boolean createDefault)
        {
            TrackingProfile trackingProfile = null;

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.GetTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetTrackingProfile,
                    TrackingParameterName.TypeFullName),
                    workflowType.FullName, AdoDbType.String);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetTrackingProfile,
                    TrackingParameterName.AssemblyFullName),
                    workflowType.Assembly.FullName, AdoDbType.String);

                if (profileVersion != emptyVersion)
                {
                    AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                        TrackingCommandName.GetTrackingProfile,
                        TrackingParameterName.Version),
                        profileVersion.ToString(), AdoDbType.String);
                }
                else
                {
                    AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                        TrackingCommandName.GetTrackingProfile,
                        TrackingParameterName.Version),
                        null, AdoDbType.String);
                }

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetTrackingProfile,
                    TrackingParameterName.CreateDefault),
                    createDefault, AdoDbType.Boolean);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetTrackingProfile,
                    TrackingParameterName.TrackingProfile),
                    AdoDbType.Cursor, ParameterDirection.Output);


                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        trackingProfile = buildProfileFromXml(_valueReader.GetString(dataReader, 0));
                }
            }

            return trackingProfile;
        }

        /// <summary>
        /// Helper method used to retrieve the tracking profile for a specified
        /// workflow instance from the tracking store.
        /// </summary>
        /// <param name="instanceId">The <see cref="Type" /> of the workflow
        /// <see cref="Guid" /> representing the workflow instance's type.
        /// </param>
        /// <returns>
        /// A <see cref="TrackingProfile" /> for the specified workflow instance.
        /// </returns>
        private TrackingProfile getProfile(Guid instanceId)
        {
            TrackingProfile trackingProfile = null;

            using (DbCommand dbCommand = CreateCommand(_nameResolver.ResolveCommandName(TrackingCommandName.GetTrackingProfile), CommandType.StoredProcedure))
            {
                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetInstanceTrackingProfile,
                    TrackingParameterName.InstanceId),
                    instanceId, AdoDbType.Guid);

                AddParameter(dbCommand, _nameResolver.ResolveParameterName(
                    TrackingCommandName.GetTrackingProfile,
                    TrackingParameterName.TrackingProfile),
                    AdoDbType.Cursor, ParameterDirection.Output);


                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                        trackingProfile = buildProfileFromXml(_valueReader.GetString(dataReader, 0));
                }
            }

            return trackingProfile;
        }

        private static TrackingProfile buildProfileFromXml(String trackingProfileXml)
        {
            if (String.IsNullOrEmpty(trackingProfileXml))
                return null;

            using (StringReader StringReader = new StringReader(trackingProfileXml))
            {
                return new TrackingProfileSerializer().Deserialize(StringReader);
            }
        }

        private static String buildXmlFromProfile(TrackingProfile trackingProfile)
        {
            if (trackingProfile == null)
                return null;
            
            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            {
                new TrackingProfileSerializer().Serialize(stringWriter, trackingProfile);

                return stringBuilder.ToString();
            }
        }
    }
}
