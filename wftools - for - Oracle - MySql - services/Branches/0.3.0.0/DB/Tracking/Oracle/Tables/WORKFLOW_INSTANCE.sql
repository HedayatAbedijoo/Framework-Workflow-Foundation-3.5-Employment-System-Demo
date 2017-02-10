CREATE TABLE WORKFLOW_INSTANCE
(
	WORKFLOW_INSTANCE_ID NUMBER(18) NOT NULL
	,INSTANCE_ID CHAR(36) NOT NULL
	,CONTEXT_GUID CHAR(36) NOT NULL
	,CALLER_INSTANCE_ID CHAR(36) NULL
	,CALL_PATH VARCHAR2(400) NULL
	,CALLER_CONTEXT_GUID CHAR(36) NULL
	,CALLER_PARENT_CONTEXT_GUID CHAR(36) NULL
	,WORKFLOW_TYPE_ID NUMBER(18) NOT NULL
	,INITIALISED_DATE_TIME DATE NOT NULL
	,DB_INITIALISED_DATE_TIME DATE DEFAULT SYS_EXTRACT_UTC(SYSTIMESTAMP) NOT NULL
	,END_DATE_TIME DATE NULL
	,DB_END_DATE_TIME DATE NULL
);

ALTER TABLE WORKFLOW_INSTANCE ADD CONSTRAINT WORKFLOW_INSTANCE_PK PRIMARY KEY ( WORKFLOW_INSTANCE_ID );

CREATE INDEX WORKFLOW_INSTANCE_IDX01 ON WORKFLOW_INSTANCE ( INSTANCE_ID, CONTEXT_GUID );
CREATE INDEX WORKFLOW_INSTANCE_IDX02 ON WORKFLOW_INSTANCE ( INSTANCE_ID, WORKFLOW_TYPE_ID );

CREATE OR REPLACE TRIGGER WORKFLOW_INSTANCE_ID_TRG
BEFORE INSERT ON WORKFLOW_INSTANCE
REFERENCING OLD AS OLD NEW AS NEW 
FOR EACH ROW 
BEGIN
  SELECT
    WORKFLOW_INSTANCE_ID_SEQ.NEXTVAL
  INTO
    :NEW.WORKFLOW_INSTANCE_ID
  FROM DUAL;
END;
/