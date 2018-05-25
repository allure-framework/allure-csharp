@labels @core @epic:v1.2 @owner:Vasya
Feature: Labels

@ui @story:accounting @123 @tms:234 @package:com.company.accounting @class:main @method:getLedger @tag1
Scenario: [v1.2 accounting] [ui.core] Selenium test 1

@api @blocker @567 @999999
Scenario: [v1.2] [api.core] Api test 1

@api @create @link:http://example.org 
Scenario: [v1.2] [api.core.create] Api test 2

@api @update 
Scenario: [v1.2] [api.core.update] Api test 3

@update @story:accounting
Scenario: [v1.2 accounting] [core.update] Update test

@epic:v.2.0 @story:security @package:com.company.security @class:main @method:getACL
Scenario: [v1.2 / v.2.0 security] [core.update] [com.company.security.main.getACL] Get ACL test