@data
Feature: Step arguments
 
Scenario: Table arguments
	Given Step with table
		|1|2|
	
	Given Step with table
		|1|2|3|
	
	Given Step with table
		| name | surname | gender | age |
		| John | Smith   | male   |     |

	Given Step with table
		| name         | surname | gender | age |
		| John         | Smith   | male   | 30  |
		| "Mary","Ann" | Jane;   | female | 25  |
		|              |         |        |     |
		| Eric         | Cartman | ,      | ,,  |
