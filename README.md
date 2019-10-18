# santedb-bis
SanteDB BI Services

## About the BI Services

SanteDB's primary APIs are designed primarily for shipping and accessing objects in a traditional OLTP method. This process involves querying data from the primary configured data store, mapping those objects to the information model, and then sending the result to a client for use in clinical care applications.

This method is very useful for interacting with the primary data store, however it is not well-suited for BI functions such as reporting, analysis, and mass data extraction. The BI services plugin provides a convenient mechanism for expressing queries, views, reports, and pivots of data from any of the connected SanteDB data stores (clinical data, audits, data marts, etc.) as JSON or XML objects. 
