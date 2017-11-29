@data
Feature: Step arguments
 
Scenario: Table arguments

	# no params
	Given Step with table
		|1|2|
	
	# no params
	Given Step with table
		|1|2|3|

	# 2 columns with header match, 1 row: process as param value wihout header (width = 10)
	Given Step with table
		| attribute | value |
		| width     | 10    |
		
	# 1 row: process as param value with header (name = John, surname = Smith, ...)
	Given Step with table
		| name | surname | gender | age |
		| John | Smith   | male   |     |
	
	# 2 column with header match, N rows: process as param value wihout header (width = 10, length = 20, height = 5)
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
