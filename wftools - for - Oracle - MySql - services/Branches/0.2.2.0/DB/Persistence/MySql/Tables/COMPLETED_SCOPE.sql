DROP TABLE IF EXISTS COMPLETED_SCOPE;
CREATE TABLE COMPLETED_SCOPE
(
	INSTANCE_ID CHAR(36) NOT NULL, 
	COMPLETED_SCOPE_ID CHAR(36) NOT NULL, 
	STATE BLOB NOT NULL, 
	MODIFIED DATETIME NOT NULL, 
	PRIMARY KEY (COMPLETED_SCOPE_ID)
);