# COSC2640A3
## COSC2640 Cloud Computing Assignment 3

#### Instruction to get started

Install the following tools and apps:

* MS SQL Server 2019 + LocalSQL (during installation, select Windows Authentication)
* SQL Server Management Studio
* Visual Studio 2019 (or JetBrains Rider)
* Postman
* Visual Studio Code (or JetBrains WebStorm)
* Redis (by downloading the installation file)
* Another Redis Desktop Manager
* Node.js
* Git client (eg. SourceTree)

MS SQL Server should be installed first to not getting any error during installation.<br />
While installing Visual Studio, select the following:
* ASP.NET and Web development
* Data storage and processing
* .NET Core cross-platform development

###### In backend .NET app and database:

First thing first, fetch and pull everything from git to local machine.

***Set up relational database***
* Open SQL Server Management Studio, enter Server Name `(localdb)\MSSQLLocalDB`, select *Windows Authentication* and connect.
* Create a database name `COSC2640A3`
* Launch Visual Studio, open the database project from `~\Database\COSC2640A3.sln`
* Double click the `.publish` file, Generate script then click execute button (top left)
* **DON'T** modify schema directly in SQL Server Management Studio
* Make changes in the database project then publish it instead

***Set up the React app***
* Launch Visual Studio Code, open the React project from `~\WebApp`
* Open the `Command Prompt` in VS Code run:
> $ npm install -g yarn <br/>
> $ yarn install <br /><br />
> $ yarn run <br/> or <br />
> $ npm run
* Voila! start coding.

***Set up the .NET API***
* Launch Visual Studio, open project from `~\COSC2640A3\COSC2640A3.sln`
* Explore and start coding.

## Remember:
### Create a git branch before making any changes in codes
