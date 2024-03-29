/*Users*/
  CREATE TABLE USERS
     (ID INTEGER NOT NULL PRIMARY KEY IDENTITY(1,1), 
	  EMAIL VARCHAR(256) DEFAULT NULL, 
	  EMAILCONFIRMED BIT, 
	  PASSWORDHASH VARCHAR(MAX), 
	  SECURITYSTAMP VARCHAR(MAX), 
	  PHONENUMBER  VARCHAR(MAX), 
	  PHONENUMBERCONFIRMED BIT, 
	  TWOFACTORENABLED  BIT, 
	  LOCKOUTENDDATEUTC  DATETIME DEFAULT NULL, 
	  LOCKOUTENABLED  BIT, 
	  ACCESSFAILEDCOUNT INTEGER, 
	  USERNAME VARCHAR(256)
   )
  
  CREATE UNIQUE INDEX USERS_INDEX ON USERS (USERNAME) 

  CREATE UNIQUE INDEX USERS_PK ON USERS (ID) 
  

 /*Roles*/

  CREATE TABLE ROLES 
   ( ID INTEGER NOT NULL PRIMARY KEY IDENTITY(1,1), 
	 NAME VARCHAR(256)
   ) 

  CREATE UNIQUE INDEX PK_ROLES ON ROLES (ID) 
  

 
 /*UserLogins */

  CREATE TABLE USERLOGINS
   ( USERID INTEGER NOT NULL, 
	 LOGINPROVIDER VARCHAR(128), 
	 PROVIDERKEY VARCHAR(128)
   )

  CREATE UNIQUE INDEX PK_USERLOGINS ON USERLOGINS (LOGINPROVIDER, PROVIDERKEY, USERID) 
 
  ALTER TABLE USERLOGINS ADD CONSTRAINT FK_USERLOGINS$USERS FOREIGN KEY (USERID)
	  REFERENCES USERS (ID) ON DELETE CASCADE;


/* User Claims */

  CREATE TABLE USERCLAIMS
   (  ID INTEGER NOT NULL PRIMARY KEY IDENTITY(1,1), 
	  USERID INTEGER NOT NULL, 
	  CLAIMTYPE VARCHAR(MAX), 
	  CLAIMVALUE VARCHAR(MAX)
   ) 

  CREATE UNIQUE INDEX PK_USERCLAIMS ON USERCLAIMS (ID) 

  ALTER TABLE USERCLAIMS ADD CONSTRAINT FK_USERCLAIMS$USERS FOREIGN KEY (USERID)
	  REFERENCES USERS (ID) ON DELETE CASCADE;



/* User Roles */

  CREATE TABLE USERROLES
    (
	 USERID INTEGER NOT NULL, 
	 ROLEID INTEGER NOT NULL
   ) 

  CREATE UNIQUE INDEX PK_USERROLES ON USERROLES (USERID, ROLEID) 
  
  ALTER TABLE USERROLES ADD CONSTRAINT FK_USERROLES$ROLES FOREIGN KEY (ROLEID)
	  REFERENCES ROLES (ID) ON DELETE CASCADE;
  ALTER TABLE USERROLES ADD CONSTRAINT FK_USERROLES$USERS FOREIGN KEY (USERID)
	  REFERENCES USERS (ID) ON DELETE CASCADE;
