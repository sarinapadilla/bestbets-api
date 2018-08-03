Feature: test bestbets controller get method

  Background:
  * url apihost

  Scenario: get the live english bestbets for Visuals Online
    Given path 'live', 'en', 'visuals+online'
    When method get
    Then status 200
    

#* def first = response[0]
#
#Given path 'users', first.id
#When method get
#Then status 200
#
#Scenario: create a user and then get it by id
#
#* def user =
#"""
#{
#  "name": "Test User",
#  "username": "testuser",
#  "email": "test@user.com",
#  "address": {
#    "street": "Has No Name",
#    "suite": "Apt. 123",
#    "city": "Electri",
#    "zipcode": "54321-6789"
#  }
#}
#"""
#
#Given url 'https://jsonplaceholder.typicode.com/users'
#And request user
#When method post
#Then status 201
#
#* def id = response.id
#* print 'created id is: ' + id
#
#Given path id
## When method get
## Then status 200
## And match response contains user

