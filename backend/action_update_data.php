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
$uname = $_POST['username'];
$opis = 	$_POST['description'];
$avatar =	$_POST['avatar'];
$karta = 	$_POST['card'];

$email = strip_tags($email);
$uname = strip_tags($uname);
$opis = strip_tags($opis);
$avatar = strip_tags($avatar);
$karta = strip_tags($karta);

$query = "
		UPDATE users
		SET userName = '$uname',
			userDescription = '$opis',
			userAvatar = '$avatar',
			userCard = '$karta'
		WHERE userEmail = '$email'
	";

$result = mysqli_query($dblink, $query);

$row = mysqli_fetch_row($result);
if($row){
	$dataArray = array('success' => true, 'error' => '');

} 
else{
	$dataArray = array('success' => false, 'error' => 'error ');

}

header('Content-Type: application/json');

echo json_encode($dataArray);
?>