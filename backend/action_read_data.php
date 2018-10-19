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

$email = $_POST['email'];
$upass = $_POST['password'];

$email = strip_tags($email);
$upass = strip_tags($upass);

$password = hash('sha256', $upass);

$query = "SELECT userName, userDescription, userAvatar, userCard FROM users WHERE userEmail='$email' AND userPass='$password' ";

$result = mysqli_query($dblink, $query);

$row = mysqli_fetch_row($result);
if($row){
	$dataArray = array($row[0],$row[1],$row[2],$row[3]
);

} else{
	$dataArray = array(
    "ResultCode" => 2,
    "Message" => "Wrong username or password"
);
}

header('Content-Type: application/json');

echo json_encode($dataArray);
?>