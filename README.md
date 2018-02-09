# SitecoreKraken
Kraken Image Optimization integration into Sitecore MediaLibrary Image Upload pipeline

1. This module can be directly used with Sitecore v7.5, v8.0, v8.1 and v8.2 (Not yet tested with v9); Just make sure the Sitecore fields are created with same names as in code. Let me try to include the Sitecore package too, just in case.

2. The App_Config include file has the cache time and the Sitecore item ID

3. The code now has the ability to default the "Alt" text with the file name (with extension) to avoid empty "Alt" field for the images

4. You will need to have paid version of Kraken to use the Kraken Cloud. The images post-compression are downloaded from this cloud. The free version does not store images in the Cloud and so you will not get a vaild URL for image download.

5. If you are having a CM/CD environment, please make sure that the module is used only in CM
