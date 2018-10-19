<?php
$dbuser = 'root';
$dbpassword = 'root';
$db = 'gameserver';
$dbhost = 'localhost';
$dbport = 3306;

$dblink = mysqli_init();
$dbconnection = mysqli_real_connect($dblink, $dbhost, $dbuser, $dbpassword, $db, $dbport);

if($dbconnection) {
print ("success");
}
else{
	die("connection failed" . mysql_error());
}


$RecEmail = $_POST['RecEmail'];
$SenEmail = $_POST['SenEmail'];

$RecEmail=strip_tags($RecEmail);
$SenEmail=strip_tags($SenEmail);

$query = "DELETE FROM friendsreq WHERE reciever='$RecEmail' AND sender='$SenEmail' ";

$result = mysqli_query($dblink, $query);

$row = mysqli_fetch_row($result);
if(!$row){
	$dataArray = array('success' => false, 'error' => 'user do not exists ');

} 
else
{

	}

header('Content-Type: application/json');

echo json_encode($dataArray);
?>