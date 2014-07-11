<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="">
    <meta name="author" content="">

    <title>Landing Page Template for Bootstrap</title>

    <!-- Bootstrap core CSS -->
    <link href="//maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css" rel="stylesheet">
    <link href="//maxcdn.bootstrapcdn.com/font-awesome/4.1.0/css/font-awesome.min.css" rel="stylesheet">

    <!-- Custom Google Web Font -->
    <!--<link href='http://fonts.googleapis.com/css?family=Lato:100,300,400,700,900,100italic,300italic,400italic,700italic,900italic' rel='stylesheet' type='text/css'>-->

    <!-- Add custom CSS here -->
    <link href="css/timeline.css" rel="stylesheet"/>
    <link href="css/page.css" rel="stylesheet"/>
    <link rel="stylesheet" type="text/css" href="css/sunlight.default.css" />

</head>

<body ng-app="virtualObjectsApp">
    
    <div class="container hidden-xs">
        <?php include 'pagecore.php' ?> 
    </div>
    
    <div class="hidden-lg hidden-sm hidden-md">
        <?php include 'pagecore.php' ?> 
    </div>
    
    <?php include 'views.php' ?>    
    
    <!-- JavaScript -->
    <script src="//cdnjs.cloudflare.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular.js/1.2.18/angular.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/angular.js/1.2.18/angular-route.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.2.0/js/bootstrap.min.js"></script>
    <script src="js/virtualObjectsApp.js"></script>
    <script type="text/javascript" src="js/sunlight-min.js"></script>
    <script type="text/javascript" src="js/lang/sunlight.csharp-min.js"></script>
    
</body>

</html>
