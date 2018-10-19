<?php
$dbuser = 'root';
$dbpassword = 'root';
$db = 'gameserver';
$dbhost = 'localhost';
$dbport = '3306';

$dblink = mysqli_init();
$dbsucces = mysqli_real_connect($dblink, $dbhost, $dbuser, $dbpassword, $db, $dbport);

if($dbsucces) {
print ("success");
}
else{
	die("connection failed" . mysql_error());
}

?>