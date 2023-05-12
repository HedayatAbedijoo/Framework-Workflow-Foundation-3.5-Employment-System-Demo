# Wrokflow Engine and State Machine Framework
### What we will have after using this workflow framework?
After using this workflow framework and designing our workflow using WF state machine, in our system we will have a Cartable page. Each user, after logging to the application and based on his role, can see different records inside the Cartable’s Grid View that represents his tasks. By clicking on each row, the associated page will be opened, and based on actions related to that State, User will be able to do some activities(like confirm state, cancel, send to next expert or etc) and finish his job. After finishing the current task, current record will be removed from his Cartable and another record automatically will create for the next user (or the next step).
the features of this workflow framework:
Custom activities to facilitate use of State Machine of Workflow Foundation 3.5
Sending and receiving business objects between host application and workflow
Enabling workflow to transit by sending text command
Transiting to the next steps using two main ways. Direct command or dynamic business condition via functions
Implementing sub workflows to expose fork/join flows
Changing transit condition functions without doing workflow versioning
 
### What are the basic required steps and information?
Actually this framework is such kind of long time running workflow which is using Microsoft workflow framework 3.5 state machine.
To implement our Business Process Management, after analyzing, we need to design all flow steps using this workflow framework and its controls. While we are generating workflow files, we have to specify the relation between each state of workflow with proper web page of our application.
Then it is turn to create all database basic information regarding with Roles, Cartable Table, Workflow Store Procedures and Tables, Resources and etc.
 
###  Let’s take a look at the different projects inside workflow framework
Inside the solution you can see 4 projects that are responsible for running workflow:
• HA.CartableService 

• HA.Workflow.Activities

• HA.Workflow.Services

• Workflow

Following each project will be explained.
### CartableService
In this project you can find two classes named, WorkflowRuntimeHelper and WorkflowManager which are responsible to connect workflow layer to host web application.
**HA.Workflow.Activities**

All the custom activities which have been designed to facilitate our workflows are located inside this project. ( like custom IfElse activity).
**HA.Workflow.Service**

All the base classes, Interfaces and other infrastructure controls and activities with regard to workflow framework are located inside this project.
Workflow
This is the project that hosts all the State Machine Workflow files that have been designed by WF 3.5.
 
### Essential Services to run workflow framework
**1- Persistence Service** : This is a fundamental microsoft service to persist and rehydrate workflow data form database. The default database which is working with this service is SQL Server. By implementing custom provider we can persist the whole workflow with other databases like Oracle, MySQL and etc.

**2- ExternalDataExchange Service:** For sending and receiving data between the host application and workflow layer, this service is necessary.

**3- ManualWorkflowScheduler Service:** Running this service for implementing WF 3.5 inside web applications is necessary. This service guarantees that all the workflow staffs are going to be executed on one thread of Request/Response. This service receives the current thread of page processing, try to run all workflow jobs and return the result and then releases the threat to continue page life cycle processing.

**4- CustomConditions Service :** One of the important features of this framework is, executing dynamic condition on each transition based on the current business object. We can write a custom function that will run on a transition. If this functions can do its job properly and return true, the state of workflow can transit to the next one.
This service should be enabled inside the Global.asax.

**5- SubWorkflow Service:** As you can imagine from it’s name, this service is responsible to run and handle all sub workflows. This service should be enabled inside the Global.asax.

### Database Schema
**1- Microsoft Tables and Store Procedure to run WF 3.5**

Tables:
* CompletedScope 
* InstanceState
Store Procedure:
* DeleteCompletedScope
* InsertCompletedScope
* InsertInstanceState
* RetrieveAllInstanceDescriptions
* RetrieveANonblockingInstanceStateId
* RetrieveCompletedScope
* RetrieveExpiredTimerIds
* RetrieveInstanceState
* RetrieveNonblockingInstanceStateIds
* UnlockInstanceState

**2- Workflow state table:** “wfActitvityState” is responsible to create connection between each state with related web page.

**3- Workflow file state table:** “wfWorkflowStatus” is responsible to maintain each workflow file status whether it is run, idled or completed.

**4- Cartable table: this table maintains all tasks information.**

In the enclosed file you can find a demo sample that contains the source code, database scripts and also adapters to compatible workflow with Oracle and MySQL.
