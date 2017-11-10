@data
Feature: Step arguments
 
Scenario: Table arguments

	# no params
	Given Step with table
		|1|2|
	
	# no params
	Given Step with table
		|1|2|3|
	
	# 1 row: process as param value
	Given Step with table
		| name | surname | gender | age |
		| John | Smith   | male   |     |
	
	# 2 column with header match: process as param value
	Given Step with table
		| attribute | value |
		| width     | 10    |
		| length    | 20    |
		| height    | 5     |
	
	# 2 column without header match: process as csv attachment
	Given Step with table
		| param  | value |
		| width  | 10    |
		| length | 20    |
		| height | 5     |

	# matrix: process as csv attachment
	Given Step with table
		| name         | surname | gender | age |
		| John         | Smith   | male   | 30  |
		| "Mary","Ann" | Jane;   | female | 25  |
		|              |         |        |     |
		| Eric         | Cartman | ,      | ,,  |
