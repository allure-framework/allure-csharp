Feature: Step arguments

  @passed
  Scenario: Table arguments

	# no params
    Given Step with table
      | 1 | 2 |
	
	# no params
    Given Step with table
      | 1 | 2 | 3 |

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

  @passed
  Scenario: Multiline arguments

    Given Step with params: 123 
		"""
		<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">
		  <entity name="xts_consumptiontaxdetail">
			<attribute name="xts_effectivestartdate" />
			<attribute name="xts_consumptiontaxid" />
			<attribute name="xts_rate" />
			<order attribute="xts_effectivestartdate" descending="true" />
			<filter type="and">
			  <condition attribute="xts_effectivestartdate" operator="on-or-before" value="@vehiclePrice.xts_effectivestartdate" />
			  <condition attribute="xts_consumptiontaxid" operator="eq" value="@vehiclePriceDetail.xts_consumptiontax1id" />
			</filter>			
		  </entity>
		</fetch>
		"""
