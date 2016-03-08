# Getting started with the **PhotoSharingApp** sample

There are two ways to explore the **PhotoSharingApp** sample:

 1. You can run the UWP app with a dummy service. This allows you to explore some basic features of the app without the need to set up the Azure service.

 2. You can set up your own Azure App Service, which will be your app backend for storing and retrieving real data.

## Pre-requisites

 1. [Visual Studio 2015](https://www.visualstudio.com/downloads/download-visual-studio-vs) with *Universal Windows App Development Tools* and *Web Developer Tools* installed. Also ensure that you have the latest updates.

 ![Visual Studio Features](Images/VisualStudio-Features.jpg)
 2. Latest [Azure SDK](https://azure.microsoft.com/en-us/downloads/).

#### Launching the app with the dummy service

The dummy service is a service implementation that loads static data into the app for exploring and testing purposes.

In Visual Studio, make sure to have *Debug* mode enabled. By default, *PhotoSharingApp.Universal* is the StartUp project, so you can Start Debugging (F5) or Start Without Debugging (Ctrl+F5) to explore the app connected to the dummy service.

**Note** : When you first load the solution, you may run into numerous warnings and errors in the Error List. This is because the NuGet packages are not downloaded and dependencies are not yet built. First *Clean* the solution and then *Build* it to get rid of all those errors.

## Setting up the Azure backend service

You need an [Azure account](https://azure.microsoft.com) to create an Azure App Service, DocumentDB, and Blob storage. If you do not have an Azure account already, you can sign up for a free one-month trial [here](https://azure.microsoft.com).

#### Create Azure Mobile App with Authentication

 1. Create an Azure Mobile App as described [here](https://azure.microsoft.com/documentation/articles/app-service-mobile-dotnet-backend-how-to-use-server-sdk/#create-app). Follow Steps 1 to 4 only, and stop after you have Clicked “Create”.
  - After the Mobile App is provisioned, you will have the mobile app URL available for use. Note this URL (it will look like *https://contoso.azurewebsites.net*).
  - Enter this URL (use the *https* version) for `string AzureAppServiceBaseUrl` in the file located at  [PhotoSharingApp\PhotoSharingApp.Universal\ServiceEnvironments\ServiceEnvironment.cs](PhotoSharingApp/PhotoSharingApp.Universal/ServiceEnvironments/ServiceEnvironment.cs#L25).

 2. Set up your Mobile App to accept authenticated users.
  - The **PhotoSharingApp** code sample allows user authentication via Microsoft Account, Facebook, Twitter, and Google. To experience the full functionality of the sample, enable at least 1 means of authentication in [Azure Portal](https://portal.azure.com/) at *Mobile App -> Settings -> Authentication/Authorization*. (**Note**: Do not use *Mobile authentication* under settings).
      - [Microsoft Account configuration](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-microsoft-authentication/).
      - [Facebook configuration](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-facebook-authentication/).
      - [Twitter configuration](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-twitter-authentication/).
      - [Google configuration](https://azure.microsoft.com/documentation/articles/app-service-mobile-how-to-configure-google-authentication/).
 - Ensure that you set "Allow request (no action)" when the request is not authenticated before you click *Save*.

![Allow request (no action)](Images/Authentication-NoAction.jpg)

**Note** : Different Authentication providers have different token expiration times. Keep that in consideration if you need to refresh tokens for continued access to the authenticated service.

#### Create Azure Blob storage

 1. Create an Azure storage account following the instructions at [Create a storage account](https://azure.microsoft.com/documentation/articles/storage-create-storage-account/).
 2. At the above link, navigate to the "View and copy storage access keys" section. Note the Storage Account Name and one of the Access Keys, and enter these values in [PhotoSharingApp\PhotoSharingApp.AppService.Shared\Context\EnvironmentDefinition.cs](PhotoSharingApp/PhotoSharingApp.AppService.Shared/Context/EnvironmentDefinition.cs)
  - `string StorageAccountName`
  - `string StorageAccessKey`

#### Create DocumentDB account, database, and collection

 1. [Create a DocumentDB account](https://azure.microsoft.com/documentation/articles/documentdb-create-account/).
 2. After creation, collect the values below from the Azure Portal and input these values at the following locations in [PhotoSharingApp\PhotoSharingApp.AppService.Shared\Context\EnvironmentDefinition.cs](PhotoSharingApp/PhotoSharingApp.AppService.Shared/Context/EnvironmentDefinition.cs):
  - The DocumentDbStorage.EndpointUrl property setting - *DocumentDB account -> Keys -> URI* (example: *https://contoso-documentdb.documents.azure.com:443/*)
  - The DocumentDbStorage.AuthorizationKey property setting - *DocumentDB account -> Keys -> Primary Key*

The DocumentDB client can programatically create databases and collections, and when the service starts up it will create these for you. There are default values already configured for your DocumentDB database and collection, but you can change these if you want to in the [EnvironmentDefinition.cs](PhotoSharingApp/PhotoSharingApp.AppService.Shared/Context/EnvironmentDefinition.cs#L25) file, and let the service to create them for you.
Or you can [create a DocumentDB database](https://azure.microsoft.com/documentation/articles/documentdb-create-database/) and [create a DocumentDB collection](https://azure.microsoft.com/documentation/articles/documentdb-create-collection/) on your own and update the DefaultEnvironmentDefinition settings with your database and collection IDs.  The service will not overwrite an existing database or collection; it creates a new one only if there is no existing one with a matching id.
  - The DocumentDbStorage.CollectionId property setting - *DocumentDB account -> Databases -> Collections*
  - The DocumentStorage.DatabaseId property setting - *DocumentDB account -> Databases*
![Example of DocumentDB account](Images/DocumentDB-Names.jpg)

#### Create NotificationHub for Push Notifications

 1. Follow Steps 8 and 9 of "Register your app for the Windows Store" on this [page](https://azure.microsoft.com/documentation/articles/notification-hubs-windows-store-dotnet-get-started/) to get the Client Secret and Package SID of your **PhotoSharingApp**.
 2. Follow "Configure your notification hub" section on the same page. Enter the Client Secret and Package SID for Windows notifications settings which you obtained in the above step.
 3. Note the DefaultFullSharedAccessSignature connection string and Notification Hub name and enter them at [PhotoSharingApp\PhotoSharingApp.AppService.Shared\Context\EnvironmentDefinition.cs](PhotoSharingApp/PhotoSharingApp.AppService.Shared/Context/EnvironmentDefinition.cs#L25)
  - ```string HubName```
  - ```string HubFullSharedAccessSignature```

##### Test app service locally

In Visual Studio, right-click on the PhotoSharingApp.AppService project and select *Set as StartUp Project*. Press *Ctrl+F5* or select *Debug* > *Start Without Debugging* to start the App Service locally.

Now set the project *PhotoSharingApp.Universal* as *StartUp Project* and launch the app in *Debug* mode. Within the app, navigate to the *Debug* page from the navigation panel of the app, disable the *Use Photo Dummy Service* switch, and select *http://localhost:XXXX/* as service endpoint from the dropdown list to test the connection with the service.

If the app has properly connected to the service, you should see a green indicator under the Service Connection Status.  If there were any issues connecting to the service the indicator will be red, please check that you have properly followed the directions involving the service deployment. (**Note**: *If the service code is not deployed to Azure (see section Deploy service to Azure and connect the app) you may get an error since service selection defaults to your set mobile service URL by default.*)

#### Deploy service to Azure and connect the app

Download the Mobile App Service publishing profile from the Azure portal (*Get Publish Profile*). Right click on *PhotoSharingApp.AppService* project *-> Select Publish -> Profile -> Import -> Browse to the downloaded Mobile App publishing profile and select it -> Click OK -> Publish*. Refer to "How to: Publish the server project" section at this [page](https://azure.microsoft.com/documentation/articles/app-service-mobile-dotnet-backend-how-to-use-server-sdk/) for more details.

Once published successfully, your **PhotoSharingApp** can now be used with the Azure App Service backend!

## Application Insights

Both service and app have been prepared to support [Application Insights](https://azure.microsoft.com/services/application-insights/) for telemetry data.

Depending on whether you want both app and service to send telemetry or one of the projects only, you will need to create one or two instances of Application Insights in Azure:
- Go to [Azure Portal](https://portal.azure.com/#create/Microsoft.AppInsights).
- Select *Windows Store Application* and enter a name for the Application Insights instance.
- Go ahead and create the instance and copy the instrumentation key

To enable the app to send telemetry to the Application Insights instance you just created, you need to paste the API key into *InstrumentationKey* of your environment definition (in [ServiceEnvironment.cs](PhotoSharingApp/PhotoSharingApp.Universal/ServiceEnvironments/ServiceEnvironment.cs#L25)).

To enable Application Insights in the service, paste your Instrumentation Key found in Azure portal of your *Application Insight instance -> Settings -> Properties* into *instrumentationKey* in [Web.config](PhotoSharingApp/PhotoSharingApp.AppService/Web.config#L24).
