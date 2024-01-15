using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageMentClass
{
    private static DataManager dataManager;

    private static MethodCollection methodCollection;

    private static ResourceController resourceController;

    private static GameID gameID;
    private static ServerInterFace serverInterFace;


    private static ModelLink modelLink;

    public static DataManager DataManagerClass
    {
        get
        {
            if (dataManager == null)
            {
                dataManager = new DataManager();
            }
            return dataManager;
        }
    }

    public static MethodCollection MethodCollectionClass
    {
        get
        {
            if (methodCollection == null)
            {

                methodCollection = new MethodCollection();
            }
            return methodCollection;
        }
    }

    public static ResourceController ResourceControllerClass
    {
        get
        {
            if (resourceController == null)
            {

                resourceController = new ResourceController();
            }
            return resourceController;
        }
    }

    public static GameID GameIDClass
    {
        get
        {
            if (gameID == null)
            {

                gameID = new GameID();
            }
            return gameID;
        }
    }

    public static ServerInterFace ServerInterFaceClass
    {
        get
        {
            if (serverInterFace == null)
            {

                serverInterFace = new ServerInterFace();
            }
            return serverInterFace;
        }
    }

    public static ModelLink ModelLinkClass
    {
        get
        {
            if (modelLink == null)
            {

                modelLink = new ModelLink();
            }
            return modelLink;
        }
    }
}
