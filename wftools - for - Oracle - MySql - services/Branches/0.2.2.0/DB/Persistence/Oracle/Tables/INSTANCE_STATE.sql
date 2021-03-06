CREATE TABLE INSTANCE_STATE
(
	INSTANCE_ID CHAR(36) NOT NULL
	,STATE BLOB
	,STATUS NUMBER(9) NOT NULL
	,UNLOCKED NUMBER(1) NOT NULL
	,BLOCKED NUMBER(1) NOT NULL
	,INFO NCLOB
	,MODIFIED DATE NOT NULL
	,OWNER_ID CHAR(36)
	,OWNED_UNTIL DATE
	,NEXT_TIMER DATE
);

ALTER TABLE INSTANCE_STATE ADD CONSTRAINT INSTANCE_STATE_PK PRIMARY KEY ( INSTANCE_ID );

ALTER TABLE INSTANCE_STATE ADD CONSTRAINT INSTANCE_STATE_CHK01 CHECK (STATUS IN (0, 1, 2, 3, 4));
ALTER TABLE INSTANCE_STATE ADD CONSTRAINT INSTANCE_STATE_CHK02 CHECK (UNLOCKED IN (0, 1));
ALTER TABLE INSTANCE_STATE ADD CONSTRAINT INSTANCE_STATE_CHK03 CHECK (BLOCKED IN (0, 1));