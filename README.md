<div style="margin-bottom: 1%; padding-bottom: 2%;">
	<img align="right" width="100px" src="https://dx29.ai/assets/img/logo-Dx29.png">
</div>

Dx29 Medical History
==============================================================================================================================================

[![Build Status](https://f29.visualstudio.com/Dx29%20v2/_apis/build/status/DEV-MICROSERVICES/Dx29.MedicalHistory?branchName=develop)](https://f29.visualstudio.com/Dx29%20v2/_build/latest?definitionId=67&branchName=develop)

### **Overview**

This is used for the management of the Dx29 patient information (databases). Therefore, it is not exposed to third parties but only the Dx29 application will have permissions to work with it. Furthermore, it ensures that only the user of the case (or with whom it has been shared) has access to its information.

It is used in the [Dx29 application](https://dx29.ai/) and therefore how to integrate it is described in the [Dx29 architecture guide](https://dx29-v2.readthedocs.io/en/latest/index.html).

It is programmed in C#, and the structure of the project is as follows:

>- src folder: This is made up of multiple folders which contains the source code of the project.
>>- Dx29.MedicalHistory.Web.API. In this project is the implementation of the controllers that expose the API methods.
>>- Dx29.MedicalHistory. It is this project that contains the logic to perform the relevant operations.
>>- Dx29 and Dx29.Cosmos. Used as libraries to add the common or more general functionalities used in Dx29 projects programmed in C#.
>- .gitignore file
>- README.md file
>- manifests folder: with the YAML configuration files for deploy in Azure Container Registry and Azure Kubernetes Service.
>- pipeline sample YAML file. For automatizate the tasks of build and deploy on Azure.

<p>&nbsp;</p>

### **Getting Started**

####  1. Configuration: Pre-requisites

This project doesn’t need any dependency but it accesses the data bases. Therefore, in order to run it, the file appsettings.secrets.json must be added to the secrets folder with the following information:

|  Key                 | Value               |		                                                                                |
|----------------------|---------------------|--------------------------------------------------------------------------------------|
| CaseRecords          | AppName             |Appplication name                                                                     |
| CaseRecords          | DatabaseName        |Database Name where case records are saved                                            |
| CaseRecords          | ConnectionString    |Connection string to case records database                                            |
| MedicalCase          | AppName             |Appplication name                                                                     |
| MedicalCase          | DatabaseName        |Database Name where medical cases are saved                                           |
| MedicalCase          | ConnectionString    |Connection string to medical cases database                                           |
| ResourceGroups       | AppName             |Appplication name                                                                     |
| ResourceGroups       | DatabaseName        |Database Name where resource groups are saved                                         |
| ResourceGroups       | ConnectionString    |Connection string to resource groups database                                         |
| Account              | Key                 |Secret from SQL database (encrypt)                                                    |
| Account              | Inx                 |Secret from SQL database (encrypt)                                                    |
| Records              | Key                 |Secret from SQL database (encrypt)                                                    |
| Records              | Inx                 |Secret from SQL database (encrypt)                                                    |

<p>&nbsp;</p>

####  2. Download and installation

Download the repository code with `git clone` or use download button.

We use [Visual Studio 2019](https://docs.microsoft.com/en-GB/visualstudio/ide/quickstart-aspnet-core?view=vs-2022) for working with this project.

<p>&nbsp;</p>

####  3. Latest releases

The latest release of the project deployed in the [Dx29 application](https://dx29.ai/) is: v0.15.00.

<p>&nbsp;</p>

#### 4. API references

It offers the following methods organised in different controllers:

>- Medical cases controller: CRUD Operations on medical cases ``` api/v1/MedicalCases/ ``` 
>>- To get the cases for an specific user by userId
>>>- GET request
>>>- URL: ```http://localhost/api/v1/MedicalCases/<userId>?includeDeleted=<bool> ``` (includeDeleted is optional, for getting the deleted cases or not. Default value = false)
>>>- Result: List of medical case objects, with: 
>>>>- Id
>>>>- User id owner
>>>>- Status of the case
>>>>- Patient info: an object with:Name, Gender, birdthdate and list of diagnosis diseases (string ids).
>>>>- Resource groups associated with this case. Dictionary with identfiers of the resource group as keys, and a object data as the value with: Type, Name, last update time and a dictionary with the resources of this group (with resource identifier as key, and string with the state or information as value).
>>>>- SharedBy and SharedWith: Who shared it with me and who I shared it with. First with the information of: userId, caseId, Status and last update date. Second with userId, status and last update date.
>>>>- Dates of created on and updated on.
>>>>- Method ToString() to return the userId and medical case id in string format: userId-id.
>>- To get the specific case for an specific user by userId and caseId
>>>- GET request
>>>- URL: ```http://localhost/api/v1/MedicalCases/<userId>/<caseId>?includeDeleted=<bool> ``` (includeDeleted is optional, for getting the deleted cases or not. Default value = false)
>>>- Result: Medical case object like the one in the request of get medical cases from userId.
>>- Create new medical case for userId
>>>- POST request
>>>- URL: ```http://localhost/api/v1/MedicalCases/<userId>```
>>>- Body request: Object with patient information: Name, gender, birthdate and list of diagnosed diseases.
>>>- Result: Medical case object like the one in the request of get medical cases from userId.
>>- Update medical case:
>>>- PATCH request
>>>- URL: ```http://localhost/api/v1/MedicalCases/<userId>/<caseId>```
>>>- Body request: Object with patient information: Name, gender, birthdate and list of diagnosed diseases.
>>>- Result: Medical case object like the one in the request of get medical cases from userId.
>>- To delete all the cases for an specific user by userId
>>>- DELETE request:
>>>- URL: ```http://localhost/api/v1/MedicalCases/<userId>``` (includeDeleted is optional, for getting the deleted cases or not. Default value = false)
>>>- Result request: Ok if all is ok, or bad request if any error occurs.
>>- Delete an existing medical case by userId and caseId
>>>- DELETE request
>>>- URL: ```http://localhost/api/v1/MedicalCases/<userId>/<caseId>```
>>>- Result request: Ok if all is ok, or bad request if any error occurs.
>- Medical cases shared controller: ``` api/v1/MedicalCaseShared/ ```
>>- Get Medical case shared by information:
>>>- GET request
>>>- URL: ```http://localhost/api/v1/MedicalCaseShared/<userId>/<caseId>```
>>>- Result request:
>>- Share share an specific medical case (caseId) of an user (userId) with another user (email):
>>>- POST request
>>>- URL: ```http://localhost/api/v1/MedicalCaseShared/<userId>/<caseId>```
>>>- Body request: Object with the email string and the action (accept, revoke, delete or created)
>>>- Result request: Medical case object like the one in the request of get medical cases from userId.
>>- Stop sharing medical case with a user.
>>>- PATCH request
>>>- URL: ```http://localhost/api/v1/MedicalCaseShared/<userId>/<caseId>```
>>>- Body request: Object with the email string and the action (accept, revoke, delete or created)
>>>- Result request: Ok if all is ok, or bad request if any error occurs.
>- Resource groups controller:  CRUD operations on resource groups ``` api/v1/ResourceGroups/ ```
>>- Get resource group by userId, caseId. Filter by type or name
>>>- GET request
>>>- URL: ```http://localhost/api/v1/ResourceGroups/<userId>/<caseId>?type=<resource type>&name=<resource name> ``` (type and name are optionals, if indicated the method will only return the resources that meets this filters).
>>>- Result request: Resource group object with:
>>>>- Identifier, name and type of the resource group
>>>>- The userId and the caseId associated with this resource group.
>>>>- Created on and updated on dates.
>>>>- A dictionary with the resources associated with resource identifier as key, and string with the state or information as value.
>>- Get resource group by id
>>>- GET request
>>>- URL: ```http://localhost/api/v1/ResourceGroups/<userId>/<caseId>/<groupId>``` 
>>>- Result request: Resource group object with the same items of get resource group previous request.
>>- Create new resource group:
>>>- POST request
>>>- URL: ```http://localhost/api/v1/ResourceGroups/<userId>/<caseId>?type=<resource type>&name=<resource name> ``` 
>>>- Body request: A list of resources objects with:
>>>>- Identifier and name of the resource
>>>>- Status: undefined, selected, unselected
>>>>- Dictionary of strings with the properties of the resource: path, score, error, errorMessage.
>>>>- The created on and updated on dates.
>>>- Result request: Resource group object with the same items of get resource group previous request.
>>- Update resource group.
>>>- PUT request
>>>- URL: ```http://localhost/api/v1/ResourceGroups/<userId>/<caseId>?type=<resource type>&name=<resource name> ``` 
>>>- Body request: A list of resources objects with the same items as the body request of create new resource.
>>>- Result request: Resource group object with the same items of get resource group previous request.
>>- Delete Reource Group by groupId 
>>>- DELETE request
>>>- URL: ```http://localhost/api/v1/ResourceGroups/<userId>/<caseId>/<groupId>``` 
>>>- Result request: Ok if all is ok, or bad request if any error occurs.
>- Resources controller: CRUD operations on resources ``` api/v1/Resources/ ```
>>- Get resources by type?, name?, resourceId?
>>>- GET request
>>>- URL:
``` 
http://localhost/api/v1/Resources/<userId>/<caseId>?groupType=<group type>&groupName=<group name>&
<resourceId=<resource id> 
``` 
>>>	(All queries are optional).
>>>- Result response: A dictionary with resourceGroupId.resourceGroupName as keys and list of resource objects as values with
>>>>- Identifier and name of the resource
>>>>- Status: undefined, selected, unselected
>>>>- Dictionary of strings with the properties of the resource: path, score, error, errorMessage.
>>>>- The created on and updated on dates.
>>- Get resources by groupId and resourceId
>>>- GET request
>>>- URL: ```http://localhost/api/v1/Resources/<userId>/<caseId>/<groupId>/<resourceId> ```
>>>- Result response: A resource object with:
>>>>- Identifier and name of the resource
>>>>- Status: undefined, selected, unselected
>>>>- Dictionary of strings with the properties of the resource: path, score, error, errorMessage.
>>>>- The created on and updated on dates.
>>- Get resources by type, name and resourceId
>>>- GET request
>>>- URL: ```http://localhost/api/v1/Resources/<userId>/<caseId>/<groupType>/<groupName>/<resourceId> ```
>>>- Result response: A resource object with:
>>>>- Identifier and name of the resource
>>>>- Status: undefined, selected, unselected
>>>>- Dictionary of strings with the properties of the resource: path, score, error, errorMessage.
>>>>- The created on and updated on dates.
>>- Upsert resources by userId and caseId:
>>>- PUT request
>>>- URL: ```http://localhost/api/v1/Resources/<userId>/<caseId>/```
>>>- Body request: A dictionary with resourceGroupId.resourceGroupName as keys and list of resource objects as values with
>>>>- Identifier and name of the resource
>>>>- Status: undefined, selected, unselected
>>>>- Dictionary of strings with the properties of the resource: path, score, error, errorMessage.
>>>>- The created on and updated on dates.
>>>- Result request: The same object send in body updated.
>>- Upsert resources by groupId:
>>>- PUT request
>>>- URL: ```http://localhost/api/v1/Resources/<userId>/<caseId>/<resourceGroupId>/```
>>>- Body request: List of resource objects with:
>>>>- Identifier and name of the resource
>>>>- Status: undefined, selected, unselected
>>>>- Dictionary of strings with the properties of the resource: path, score, error, errorMessage.
>>>>- The created on and updated on dates.
>>>- Result request: Resource group object with:
>>>>- Identifier, name and type of the resource group
>>>>- The userId and the caseId associated with this resource group.
>>>>- Created on and updated on dates.
>>>>- A dictionary with the resources associated with resource identifier as key, and string with the state or information as value.
>>- Upsert resources by type and name
>>>- PUT request
>>>- URL: ```http://localhost/api/v1/Resources/<userId>/<caseId>/<groupType>/>groupName>```
>>>- Body request: List of resource objects with:
>>>>- Identifier and name of the resource
>>>>- Status: undefined, selected, unselected
>>>>- Dictionary of strings with the properties of the resource: path, score, error, errorMessage.
>>>>- The created on and updated on dates.
>>>- Result request: Resource group object with:
>>>>- Identifier, name and type of the resource group
>>>>- The userId and the caseId associated with this resource group.
>>>>- Created on and updated on dates.
>>>>- A dictionary with the resources associated with resource identifier as key, and string with the state or information as value.
>>- Delete resources by groupId, resourceId:
>>>- DELETE request
>>>- URL: ```http://localhost/api/v1/Resources/<userId>/<caseId>/<groupId>?resourceId=<list of resource ids>```
>>>- Result request:Resource group object with:
>>>>- Identifier, name and type of the resource group
>>>>- The userId and the caseId associated with this resource group.
>>>>- Created on and updated on dates.
>>>>- A dictionary with the resources associated with resource identifier as key, and string with the state or information as value.
>>- Delete resources by group type, group name, resourceId:
>>>- DELETE request
>>>- URL: ```http://localhost/api/v1/Resources/<userId>/<caseId>/<groupType>/<groupName>?resourceId=<list of resource ids>```
>>>- Result request: Resource group object with:
>>>>- Identifier, name and type of the resource group
>>>>- The userId and the caseId associated with this resource group.
>>>>- Created on and updated on dates.
>>>>- A dictionary with the resources associated with resource identifier as key, and string with the state or information as value.


<p>&nbsp;</p>

### **Build and Test**

#### 1. Build

We could use Docker. 

Docker builds images automatically by reading the instructions from a Dockerfile – a text file that contains all commands, in order, needed to build a given image.

>- A Dockerfile adheres to a specific format and set of instructions.
>- A Docker image consists of read-only layers each of which represents a Dockerfile instruction. The layers are stacked and each one is a delta of the changes from the previous layer.

Consult the following links to work with Docker:

>- [Docker Documentation](https://docs.docker.com/reference/)
>- [Docker get-started guide](https://docs.docker.com/get-started/overview/)
>- [Docker Desktop](https://www.docker.com/products/docker-desktop)

The first step is to run docker image build. We pass in . as the only argument to specify that it should build using the current directory. This command looks for a Dockerfile in the current directory and attempts to build a docker image as described in the Dockerfile. 
```docker image build . ```

[Here](https://docs.docker.com/engine/reference/commandline/docker/) you can consult the Docker commands guide.

<p>&nbsp;</p>

#### 2. Deployment

To work locally, it is only necessary to install the project and build it using Visual Studio 2019. 

The deployment of this project in an environment is described in [Dx29 architecture guide](https://dx29-v2.readthedocs.io/en/latest/index.html), in the deployment section. In particular, it describes the steps to execute to work with this project as a microservice (Docker image) available in a kubernetes cluster:

1. Create an Azure container Registry (ACR). A container registry allows you to store and manage container images across all types of Azure deployments. You deploy Docker images from a registry. Firstly, we need access to a registry that is accessible to the Azure Kubernetes Service (AKS) cluster we are creating. For this purpose, we will create an Azure Container Registry (ACR), where we will push images for deployment.
2. Create an Azure Kubernetes cluster (AKS) and configure it for using the prevouos ACR
3. Import image into Azure Container Registry
4. Publish the application with the YAML files that defines the deployment and the service configurations. 

This project includes, in the Deployments folder, YAML examples to perform the deployment tasks as a microservice in an AKS. 
Note that this service is configured as "ClusterIP" since it is not exposed externally in the [Dx29 application](https://dx29.ai/), but is internal for the application to use. If it is required to be visible there are two options:
>- The first, as realised in the Dx29 project an API is exposed that communicates to third parties with the microservice functionality.
>- The second option is to directly expose this microservice as a LoadBalancer and configure a public IP address and DNS.

>>- **Interesting link**: [Deploy a Docker container app to Azure Kubernetes Service](https://docs.microsoft.com/en-GB/azure/devops/pipelines/apps/cd/deploy-aks?view=azure-devops&tabs=java)

<p>&nbsp;</p>

### **Contribute**

Please refer to each project's style and contribution guidelines for submitting patches and additions. The project uses [gitflow workflow](https://nvie.com/posts/a-successful-git-branching-model/). 
According to this it has implemented a branch-based system to work with three different environments. Thus, there are two permanent branches in the project:
>- The develop branch to work on the development environment.
>- The master branch to work on the test and production environments.

In general, we follow the "fork-and-pull" Git workflow.

>1. Fork the repo on GitHub
>2. Clone the project to your own machine
>3. Commit changes to your own branch
>4. Push your work back up to your fork
>5. Submit a Pull request so that we can review your changes

The project is licenced under the **(TODO: LICENCE & LINK & Brief explanation)**

<p>&nbsp;</p>
<p>&nbsp;</p>

<div style="border-top: 1px solid !important;
	padding-top: 1% !important;
    padding-right: 1% !important;
    padding-bottom: 0.1% !important;">
	<div align="right">
		<img width="150px" src="https://dx29.ai/assets/img/logo-foundation-twentynine-footer.png">
	</div>
	<div align="right" style="padding-top: 0.5% !important">
		<p align="right">	
			Copyright © 2020
			<a style="color:#009DA0" href="https://www.foundation29.org/" target="_blank"> Foundation29</a>
		</p>
	</div>
<div>
