﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    class ETEmailSendDefinitionTest
    {
        ETClient client;
        ETEmailSendDefinition emailSendDef;
        ETEmail email;
        ETDataExtension dataExtension;
        string desc = "Created with C# Fuel SDK";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new ETClient();
        }

        [SetUp]
        public void Setup()
        {
            //To create a email send definition we need: Email, SendableList, SendClassification
            //we will create email ,sendable data extension for SendableList

            var dataExtensionName = Guid.NewGuid().ToString();
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                Name = dataExtensionName,
                CustomerKey = dataExtensionName,
                IsSendable = true,
                SendableDataExtensionField = new ETDataExtensionColumn { Name = "SubscriberID", FieldType = DataExtensionFieldType.Number },
                SendableSubscriberField = new ETProfileAttribute { Name = "Subscriber ID", Value = "Subscriber ID" },
                Columns = new[] { 
                    new ETDataExtensionColumn { Name = "SubscriberID", FieldType = DataExtensionFieldType.Number, IsPrimaryKey = true, IsRequired = true },
                    new ETDataExtensionColumn { Name = "FirstName", FieldType = DataExtensionFieldType.Text },
                    new ETDataExtensionColumn { Name = "LastName", FieldType = DataExtensionFieldType.Text } 
                }
            };

            var result = deObj.Post();
            dataExtension = (ETDataExtension)result.Results[0].Object;
            var dataExtensionId = result.Results[0].NewObjectID;
            //create Email
            var emailName = string.Empty;
            var emailCustKey = emailName = System.Guid.NewGuid().ToString();
            var emailContent = "<b>This is a content generated by Fuel SDK C#";

            var emailObj = new ETEmail
            {
                AuthStub = client,
                Name = emailName,
                CustomerKey = emailCustKey,
                Subject = "This email is created using C# SDK",
                HTMLBody = emailContent
            };
            var emailResponse = emailObj.Post();
            Assert.AreEqual(emailResponse.Code, 200);
            Assert.AreEqual(emailResponse.Status, true);
            email = (ETEmail)emailResponse.Results[0].Object;

            

            //create the send def
            var sendDefName = Guid.NewGuid().ToString();
            var sendDefList = new ETSendDefinitionList
            {
                Name = "SendDefintion List CSharp",
                CustomerKey = dataExtension.CustomerKey,
                SendDefinitionListType = SendDefinitionListTypeEnum.SourceList,
                CustomObjectID = dataExtension.ObjectID,
                DataSourceTypeID = DataSourceTypeEnum.CustomObject
            };
            var postESDDE = new ETEmailSendDefinition
            {
                AuthStub = client,
                Name = sendDefName,
                CustomerKey = sendDefName,
                Description = desc,
                SendClassification = new ETSendClassification { CustomerKey = "Default Commercial" },
                SendDefinitionList = new[] { sendDefList },
                Email = new ETEmail { ID = emailResponse.Results[0].NewID },
            };
            var postResponse = postESDDE.Post();
            Assert.AreEqual(postResponse.Code, 200);
            Assert.AreEqual(postResponse.Status, true);

            emailSendDef = (ETEmailSendDefinition) postResponse.Results[0].Object;
        }

        [TearDown]
        public void TearDown()
        {
            var emailSendDef = new ETEmailSendDefinition
            {
                AuthStub = client,
                CustomerKey = this.emailSendDef.CustomerKey
            };
            var response = emailSendDef.Delete();

            var email = new ETEmail
            {
                AuthStub = client,
                CustomerKey = this.email.CustomerKey
            };
            var emailResponse = email.Delete();

            var de = new ETDataExtension
            {
                AuthStub = client,
                CustomerKey = this.dataExtension.CustomerKey
            };
            var deResponse = de.Delete();
        }

        [Test()]
        public void EmailSendDefinitionCreate()
        {
            Assert.AreNotEqual(emailSendDef, null);
        }

        [Test()]
        public void EmailSendDefinitionUpdate()
        {
            var updatedDesc = "Updated with C# Fuel SDK";
            var emailSendDef = new ETEmailSendDefinition
            {
                AuthStub = client,
                CustomerKey = this.emailSendDef.CustomerKey,
                Description = updatedDesc,
            };
            var response = emailSendDef.Patch();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "EmailSendDefinition updated");

            var getEmailSendDef = new ETEmailSendDefinition
            {
                AuthStub = client,
                Props = new[] { "Name", "Description", "CustomerKey" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { this.emailSendDef.CustomerKey } }
            };
            var getResponse = getEmailSendDef.Get();
            Assert.AreEqual(getResponse.Code, 200);
            Assert.AreEqual(getResponse.Status, true);
            var esd = (ETEmailSendDefinition)getResponse.Results[0];
            Assert.AreEqual(esd.Description, updatedDesc);
        }

        [Test()]
        public void EmailSendDefinitionGet()
        {
            var getEmailSendDef = new ETEmailSendDefinition
            {
                AuthStub = client,
                Props = new[] { "Name", "Description", "CustomerKey" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { this.emailSendDef.CustomerKey } }
            };
            var getResponse = getEmailSendDef.Get();
            Assert.AreEqual(getResponse.Code, 200);
            Assert.AreEqual(getResponse.Status, true);
            var esd = (ETEmailSendDefinition)getResponse.Results[0];
            Assert.AreEqual(esd.Description, desc);
        }

        [Test()]
        public void EmailSendDefinitionDelete()
        {
            var emailSendDef = new ETEmailSendDefinition
            {
                AuthStub = client,
                CustomerKey = this.emailSendDef.CustomerKey
            };
            var response = emailSendDef.Delete();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "EmailSendDefinition deleted");
        }
        
    }
}
