CREATE TABLE TRACKING_PROFILE
(
	TRACKING_PROFILE_ID NUMBER(9) NOT NULL
	,VERSION VARCHAR2(32) NOT NULL
	,WORKFLOW_TYPE_ID NUMBER(18) NOT NULL
	,TRACKING_PROFILE_XML NCLOB NULL
	,INSERT_DATE_TIME DATE DEFAULT SYS_EXTRACT_UTC(SYSTIMESTAMP) NOT NULL
);

ALTER TABLE TRACKING_PROFILE ADD CONSTRAINT TRACKING_PROFILE_PK PRIMARY KEY ( TRACKING_PROFILE_ID );

CREATE INDEX TRACKING_PROFILE_IDX01 ON TRACKING_PROFILE ( WORKFLOW_TYPE_ID, VERSION, INSERT_DATE_TIME );

CREATE OR REPLACE TRIGGER TRACKING_PROFILE_ID_TRG
BEFORE INSERT ON TRACKING_PROFILE
REFERENCING OLD AS OLD NEW AS NEW 
FOR EACH ROW 
BEGIN
  SELECT
    TRACKING_PROFILE_ID_SEQ.NEXTVAL
  INTO
    :NEW.TRACKING_PROFILE_ID
  FROM DUAL;
END;
/