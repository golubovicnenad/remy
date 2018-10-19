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
$uname = $_POST['username'];

$email = strip_tags($email);
$upass = strip_tags($upass);
$uname = strip_tags($uname);

$opis = "Kratak opis profila...";
$avatar = 1;
$karta = 1;

$password = hash('sha256', $upass);

$query = "SELECT userEmail FROM users WHERE userEmail='$email' ";

$result = mysqli_query($dblink, $query);

$row = mysqli_fetch_row($result);
if($row){
	$dataArray = array('success' => false, 'error' => 'user already exists ');

} else{
	$query2 = "INSERT INTO users(userEmail, userPass, userName, userDescription, userAvatar, userCard ) VALUES ('$email', '$password', '$uname', '$opis', '$avatar', '$karta')";

	if($result2 = mysqli_query($dblink, $query2)){
		$dataArray = array('success' => true, 'error' => '',  'email' => 
			"$email");
	}
		else{
			$dataArray = array('success' => false, 'error' => 'Could not create user, try again later.');
		}
	}

header('Content-Type: application/json');

echo json_encode($dataArray);
?>