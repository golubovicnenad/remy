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


$user1 = $_POST['user1'];
$user2 = $_POST['user2'];

$user1=strip_tags($user1);
$user2=strip_tags($user2);

$query2 = "INSERT INTO friends(user1, user2) VALUES ('$user1', '$user2')";

$result2 = mysqli_query($dblink, $query2);

$row2 = mysqli_fetch_row($result2);
if(row2){
	$dataArray = array('success' => false, 'error' => 'user do not exists ');

} else{
	
	}

header('Content-Type: application/json');

echo json_encode($dataArray);
?>