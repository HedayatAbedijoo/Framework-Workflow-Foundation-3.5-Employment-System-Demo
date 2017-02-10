CREATE TABLE TRACKING_DATA_ITEM 
(
	TRACKING_DATA_ITEM_ID NUMBER(18) NOT NULL
	,WORKFLOW_INSTANCE_ID NUMBER(18) NOT NULL
	,EVENT_ID NUMBER(18) NOT NULL
	,EVENT_TYPE CHAR(1) NOT NULL
	,FIELD_NAME VARCHAR2(256) NOT NULL
	,FIELD_TYPE_ID NUMBER(18) NOT NULL
	,DATA_STR NCLOB NULL
	,DATA_BLOB BLOB NULL
	,DATA_NON_SERIALISABLE NUMBER(1) NOT NULL
);

ALTER TABLE TRACKING_DATA_ITEM ADD CONSTRAINT TRACKING_DATA_ITEM_PK PRIMARY KEY ( TRACKING_DATA_ITEM_ID );

ALTER TABLE TRACKING_DATA_ITEM ADD CONSTRAINT TRACKING_DATA_ITEM_CHK01 CHECK (EVENT_TYPE IN ('A', 'U', 'W'));
ALTER TABLE TRACKING_DATA_ITEM ADD CONSTRAINT TRACKING_DATA_ITEM_CHK02 CHECK (DATA_NON_SERIALISABLE IN (0, 1));

CREATE INDEX TRACKING_DATA_ITEM_IDX01 ON TRACKING_DATA_ITEM ( WORKFLOW_INSTANCE_ID, EVENT_ID, EVENT_TYPE );

CREATE OR REPLACE TRIGGER TRACKING_DATA_ITEM_ID_TRG
BEFORE INSERT ON TRACKING_DATA_ITEM
REFERENCING OLD AS OLD NEW AS NEW 
FOR EACH ROW 
BEGIN
  SELECT
    TRACKING_DATA_ITEM_ID_SEQ.NEXTVAL
  INTO
    :NEW.TRACKING_DATA_ITEM_ID
  FROM DUAL;
END;
/