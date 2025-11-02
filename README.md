<p align="center">
  <img src="https://raw.githubusercontent.com/yourusername/yourrepo/main/animated-text.svg" alt="Sliding Text">
</p>
<svg xmlns="http://www.w3.org/2000/svg" width="500" height="50">
  <text x="0" y="35" font-size="24" fill="blue">
    <animate attributeName="x" from="500" to="-200" dur="5s" repeatCount="indefinite" />
    🚀 Welcome to EMEDICINE PROJECT 🚀
  </text>
</svg>


Project Introduction:
EMED project is an online medical ecommerce project where users ca place order for medicines.Project has two kinds of users which are as below:
1.Admin: Manages order approval,cancellation,add new medicines etc.
2.Customer:Can order for medicines after adding into cart and then gets an approval email after placement of order on his/her registered email

Project Setup Configurations:
Step#1:
After project clone into Visual Studio find file DatabaseCreation and run script in sql server to add database with name [EMED].
Step#2:
After database creation go to appsettings.json.Add name of your sqlserver into connection string.
Step#3:
Run the project and see if project is executed successfully.
