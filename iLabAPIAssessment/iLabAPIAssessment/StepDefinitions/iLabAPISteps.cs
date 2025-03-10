﻿using iLabAPIAssessment.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TechTalk.SpecFlow;

namespace iLabAPIAssessment.StepDefinitions
{
    [Binding]
    public sealed class iLabAPISteps
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext _scenarioContext;

        RestRequest request = null;
        RestClient client = null;

        bool isPetAvailable;

        private dynamic json;

        string catObject = null;
        public iLabAPISteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I set the required endpoint")]
        public void GivenIHaveTheRequiredEndlpoint()
        {
            client = new RestClient("https://petstore.swagger.io/v2/");
        }

        [When(@"I GET pet with id that is '(.*)'")]
        public void WhenIGETPetWithIdThatIs(int petId)
        {
            request = new RestRequest("pet/{petId}", Method.GET);

            request.AddUrlSegment("petId", petId);
        }

        [When(@"I submit a GET request to list all available pets")]
        public void WhenIGETAllAvailablePets()
        {
            request = new RestRequest("pet/findByStatus", Method.GET);

            request.AddParameter("status","available", ParameterType.QueryString);

            var response = client.Execute(request);

            json = JsonConvert.DeserializeObject(response.Content);
        }

        [When(@"I confirm the list has the name '(.*)' with category id '(.*)'")]
        public void WhenIConfirmTheListHasWithCategoryId(string name, int categoryId)
        {
            foreach (var item in json)
            {
                string a = item.category["id"];
                string b = item.category["name"];

                if (item.category["id"] == categoryId && item.category["name"] == name)
                {
                    isPetAvailable = true;
                    break;
                }
                else
                {
                    isPetAvailable = false;
                }
            }

            Assert.IsTrue(isPetAvailable,"Pet Does not Exist in the list");
        }

        [When(@"I confirm the json has the name '(.*)' with category id '(.*)'")]
        public void WhenIConfirmThejsonHasWithCategoryId(string name, int categoryId)
        {

            string a = json["id"];
            string b = json["name"];

            if (json["id"] == categoryId && json["name"] == name)
            {
                isPetAvailable = true;
            }
            else
            {
                isPetAvailable = false;
            }
            
        }

        [When(@"I set up the body and submit POST request with name '(.*)', category Id '(.*)' and status '(.*)'")]
        public void WhenISetUpTheRequiredBodyAndSubmitRequestWithNameCategoryIdAnd(string name, 
            string categoryId, string status)
        {
            request = new RestRequest("pet/", Method.POST);

            request.RequestFormat = DataFormat.Json;

            request.AddBody(new Pet() { id = categoryId, name = name, status = status });

            var response = client.Execute(request);

            json = JsonConvert.DeserializeObject(response.Content);

            Assert.IsTrue(json["name"] == name, "Pet added Assertion failed.");
        }

        [Then(@"I should see new pet on the list")]
        public void ThenIShouldSeeNewPetOnTheList()
        {
            Assert.IsTrue(isPetAvailable, "Pet did not get inserted into the list");
        }

        [When(@"I submit GET request in get pets by category id")]
        public void WhenISubmitGETRequestInGetPetsByCategoryIdAnd()
        {
            string catergoryId = json["id"];

            request = new RestRequest("pet/{petId}", Method.GET);

            request.AddUrlSegment("petId", catergoryId);

            var response = client.Execute(request);

            json = JsonConvert.DeserializeObject(response.Content);
        }

        [Then(@"I should see pet is returned from request '(.*)'")]
        public void ThenIShouldSeePetIsReturnedFromRequest(string name)
        {
            Assert.IsTrue(json["name"] == name, "Pet added Assertion failed.");
        }

    }
}
