 CREATE TABLE ACTIVITY_INSTANCE
 (
	ACTIVITY_INSTANCE_ID NUMBER(18) NOT NULL
	,WORKFLOW_INSTANCE_ID NUMBER(18) NOT NULL
	,QUALIFIED_NAME VARCHAR2(128) NOT NULL
	,CONTEXT_GUID CHAR(36) NOT NULL
	,PARENT_CONTEXT_GUID CHAR(36) NULL
	,WORKFLOW_INSTANCE_EVENT_ID NUMBER(18) NULL
);

ALTER TABLE ACTIVITY_INSTANCE ADD CONSTRAINT ACTIVITY_INSTANCE_PK PRIMARY KEY ( ACTIVITY_INSTANCE_ID );

CREATE INDEX ACTIVITY_INSTANCE_IDX01 ON ACTIVITY_INSTANCE ( WORKFLOW_INSTANCE_ID );
CREATE INDEX ACTIVITY_INSTANCE_IDX02 ON ACTIVITY_INSTANCE ( WORKFLOW_INSTANCE_ID, QUALIFIED_NAME, CONTEXT_GUID, PARENT_CONTEXT_GUID );

CREATE OR REPLACE TRIGGER ACTIVITY_INSTANCE_ID_TRG
BEFORE INSERT ON ACTIVITY_INSTANCE
REFERENCING OLD AS OLD NEW AS NEW 
FOR EACH ROW 
BEGIN
  SELECT
    ACTIVITY_INSTANCE_ID_SEQ.NEXTVAL
  INTO
    :NEW.ACTIVITY_INSTANCE_ID
  FROM DUAL;
END;
/