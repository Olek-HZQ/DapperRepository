Oracle.ManagedDataAccess NuGet Package 21.6.1 README
====================================================
Release Notes: Oracle Data Provider for .NET, Managed Driver

April 2022

This README supplements the main ODP.NET 21c documentation.
https://docs.oracle.com/en/database/oracle/oracle-database/21/odpnt/


TABLE OF CONTENTS
*New Features
*Bug Fixes
*Installation and Configuration Steps
*Installation Changes
*Documentation Corrections and Additions
*ODP.NET, Managed Driver Tips, Limitations, and Known Issues


New Features
====================================
- Oracle Identity and Access Management (IAM)
ODP.NET supports Oracle Identity and Access Management (IAM) cloud service for 
unified identity across Oracle cloud services, including Oracle Cloud Database 
Services, with the core and managed drivers. ODP.NET can use the same Oracle IAM 
credentials for authentication and authorization to the Oracle Cloud and Oracle 
cloud databases, including Autonomous Database. This capability allows single 
sign-on and for identity to be propagated to all services Oracle IAM supports 
including federated users via Azure Active Directory and Microsoft Active 
Directory (on-premises). A unified identity makes user management and account 
management easier for administrators and end users.

- Oracle Identity and Access Management (IAM) - Database Alternative Password
ODP.NET Core and managed ODP.NET support alternate database password for Oracle IAM token authentication. 
This alternate password is different from the actual user's database password. It is used for database 
token authentication with shared Oracle Autonomous Database. This capability allows token-based 
authentication to be used with a password. 

- The Secure External Password Store (SEPS) Support
The Secure External Password Store (SEPS) is the usage of a client-side wallet
for securely storing the password credentials.  Oracle Data Provider for .NET 
Core can now be configured to use the external password store.
Please read the ODP.NET Developer's Guide on how to configure and utilize SEPS.
The provided instructions are the same for ODP.NET Unmanaged, Managed, and Core.


Bug Fixes since Oracle.ManagedDataAccess NuGet Package 21.5.0
=============================================================
Bug 33681872 - UDT: LARGE UDT CAUSES ORA-06502 NUMERIC OR VALUE ERROR: RAW VARIABLE LENGTH TOO LONG
Bug 33293917 - UDT: NULLABLE DATETIME TYPES ARE NOT BEING STORED PROPERLY IN THE DATABASE
Bug 32843859 - ORA-01006: BIND VARIABLE DOES NOT EXIST ERROR OCCURS WHEN DERIVEPARAMETERS USED WITH DIFFERENT DB VERSIONS
Bug 32675064 - UDT: USING UDT SUBSTITUTION ACROSS SCHEMAS RETRIEVES UDT METADATA FOR EACH RETRIEVED INSTANCE INSTEAD OF CACHING
Bug 32634175 - UDT: ORA-01008 IS ENCOUNTERED WHEN A COLLECTION ELEMENT TYPE IS DEFINED IN ANOTHER SCHEMA 
Bug 31806772 - ORACLECOMMANDBUILDER WITH BINDBYNAME IN CONFIG CAUSES EXCEPTION
Bug 30702373 - ORA-12592: INVALID NS PACKET FORMAT : DUE TO LARGE ACCEPT DATA


Installation and Configuration Steps
====================================
The downloads are NuGet packages that can be installed with the NuGet Package Manager. These instructions apply 
to install ODP.NET, Managed Driver.

1. Un-GAC and un-configure any existing assembly (i.e. Oracle.ManagedDataAccess.dll) and policy DLL 
(i.e. Policy.4.122.Oracle.ManagedDataAccess.dll) for the ODP.NET, Managed Driver, version 4.122.21.1
that exist in the GAC. Remove all references of Oracle.ManagedDataAccess from machine.config file, if any exists.

2. In Visual Studio, open NuGet Package Manager from an existing Visual Studio project. 

3. Install the NuGet package from NuGet Gallery (nuget.org).


   From Local Package Source
   -------------------------
   A. Click on the Settings button in the lower left of the dialog box.

   B. Click the "+" button to add a package source. In the Source field, enter in the directory location where the 
   NuGet package(s) were downloaded to. Click the Update button, then the Ok button.

   C. On the left side, under the Online root node, select the package source you just created. The ODP.NET NuGet 
   packages will appear.


   From Nuget.org
   --------------
   A. In the Search box in the upper right, search for the package with id, "Oracle.ManagedDataAccess". Verify 
   that the package uses this unique ID to ensure it is the official Oracle Data Provider for .NET, Managed Driver 
   download.

   B. Select the package you wish to install.


4. Click on the Install button to select the desired NuGet package(s) to include with the project. Accept the 
license agreement and Visual Studio will continue the setup.

5. Open the app/web.config file to configure the ODP.NET connection string and connect descriptors.
Below is an example of configuring the net service aliases and connect descriptors parameters:

  <oracle.manageddataaccess.client>
    <version number="*">
      <dataSources>
        <!-- Customize these connection alias settings to connect to Oracle DB -->
        <dataSource alias="MyDataSource" descriptor="(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL))) " />
      </dataSources>
    </version>
  </oracle.manageddataaccess.client>

After following these instructions, ODP.NET is now configured and ready to use.

IMPORTANT: Oracle recommends configuring net service aliases and connect descriptors in a .NET config file to 
have the application configuration be self-contained rather than using tnsnames.ora or TNS_ADMIN. 

NOTE: ODP.NET, Managed Driver comes with one set of platform specific assemblies for Kerberos support: Oracle.ManagedDataAccessIOP.dll.

The Oracle.ManagedDataAccessIOP.dll assembly is ONLY needed if you are using Kerberos5 based external 
authentication. Kerberos5 users will need to download MIT Kerberos for Windows version 4.0.1 from 
	https://web.mit.edu/kerberos/dist/
to utilize ODP.NET, Managed Driver's support of Kerberos5.

The asssemblies are located under
      packages\Oracle.ManagedDataAccess.<version>\bin\x64
and
      packages\Oracle.ManagedDataAccess.<version>\bin\x86
depending on the platform.

If these assemblies are required by your application, your Visual Studio project requires additional changes.

Use the following steps for your application to use the 64-bit version of Oracle.ManagedDataAccessIOP.dll:

1. Right click on the Visual Studio project.
2. Select Add -> New Folder.
3. Name the folder x64.
4. Right click on the newly created x64 folder.
5. Select Add -> Existing Item.
6. Browse to packages\Oracle.ManagedDataAccess.<version>\bin\x64 under your project solution directory.
7. Choose Oracle.ManagedDataAccessIOP.dll.
8. Click the 'Add' button.
9. Left click the newly added Oracle.ManagedDataAccessIOP.dll in the x64 folder.
10. In the properties window, set 'Copy To Output Directory' to 'Copy Always'.

For x86 targeted applications, name the folder x86 and add assemblies from the 
packages\Oracle.ManagedDataAccess.<version>\bin\x86 folder.

To make your application platform independent even if it depends on Oracle.ManagedDataAccessIOP.dll, create both x64 and x86 folders with the necessary assemblies added to them.


Installation Changes
====================
The following app/web.config entries are added by including the ODP.NET, Managed Driver NuGet package to your application:

1) Configuration Section Handler

The following entry is added to the app/web.config to enable applications to add an <oracle.manageddataaccess.client> 
section for ODP.NET, Managed Driver-specific configuration:

<configuration>
  <configSections>
    <section name="oracle.manageddataaccess.client" type="OracleInternal.Common.ODPMSectionHandler, Oracle.ManagedDataAccess, Version=4.122.21.1, Culture=neutral, PublicKeyToken=89b483f429c47342" />
  </configSections>
</configuration>

Note: If your application is a web application and the above entry was added to a web.config and the same config 
section handler for "oracle.manageddataaccess.client" also exists in machine.config but the "Version" attribute values 
are different, an error message of "There is a duplicate 'oracle.manageddataaccess.client' section defined." may be 
observed at runtime.  If so, the config section handler entry in the machine.config for 
"oracle.manageddataaccess.client" has to be removed from the machine.config for the web application to not encounter 
this error.  But given that there may be other applications on the machine that depended on this entry in the 
machine.config, this config section handler entry may need to be moved to all of the application's .NET config file on 
that machine that depend on it.

2) DbProviderFactories

The following entry is added for applications that use DbProviderFactories and DbProviderFactory classes. Also, any 
DbProviderFactories entry for "Oracle.ManagedDataAccess.Client" in the machine.config will be ignored with the following 
entry:

<configuration>
  <system.data>
    <DbProviderFactories>
      <remove invariant="Oracle.ManagedDataAccess.Client" />
      <add name="ODP.NET, Managed Driver" invariant="Oracle.ManagedDataAccess.Client" description="Oracle Data Provider for .NET, Managed Driver" type="Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Version=4.122.21.1, Culture=neutral, PublicKeyToken=89b483f429c47342" />
    </DbProviderFactories>
  </system.data>
</configuration>

3) Dependent Assembly

The following entry is created to ignore policy DLLs for Oracle.ManagedDataAccess.dll and always use the 
Oracle.ManagedDataAccess.dll version that is specified by the newVersion attribute in the <bindingRedirect> element.  
The newVersion attribute corresponds to the Oracle.ManagedDataAccess.dll version which came with the NuGet package 
associated with the application.

<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <publisherPolicy apply="no" />
        <assemblyIdentity name="Oracle.ManagedDataAccess" publicKeyToken="89b483f429c47342" culture="neutral" />
        <bindingRedirect oldVersion="4.122.0.0 - 4.65535.65535.65535" newVersion="4.122.21.1" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>

4) Data Sources

The following entry is added to provide a template on how a data source can be configured in the app/web.config. 
Simply rename "MyDataSource" to an alias of your liking and modify the PROTOCOL, HOST, PORT, SERVICE_NAME as required 
and un-comment the <dataSource> element. Once that is done, the alias can be used as the "data source" attribute in 
your connection string when connecting to an Oracle Database through ODP.NET, Managed Driver.

<configuration>
  <oracle.manageddataaccess.client>
    <version number="*">
      <dataSources>
        <dataSource alias="SampleDataSource" descriptor="(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL))) " />
      </dataSources>
    </version>
  </oracle.manageddataaccess.client>
</configuration>


Documentation Corrections and Additions
=======================================
None


Known Issues and Limitations
============================
1) BindToDirectory throws NullReferenceException on Linux when LdapConnection AuthType is Anonymous

https://github.com/dotnet/runtime/issues/61683

This issue is observed when using System.DirectoryServices.Protocols, version 6.0.0.
To workaround the issue, use System.DirectoryServices.Protocols, version 5.0.1.


 Copyright (c) 2021, 2022, Oracle and/or its affiliates. 
