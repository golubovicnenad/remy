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

$RecEmail = strip_tags($RecEmail);
$SenEmail = strip_tags($SenEmail);

$query1 = "SELECT userEmail FROM users WHERE userEmail='$RecEmail'";

$result1 = mysqli_query($dblink, $query1);

$row1 = mysqli_fetch_row($result1);

if($row1){

	$query2 = "SELECT sender FROM friendsreq WHERE reciever='$RecEmail' AND sender='$SenEmail' ";

	$result2 = mysqli_query($dblink, $query2);

	$row2 = mysqli_fetch_row($result2);

	if($row2){
		$dataArray = array(
    	"ResultCode" => 2,
    	"Message" => "Req already sent!",);
	}

	else{
		$query3 = "INSERT INTO friendsreq(sender, reciever ) VALUES ('$SenEmail', '$RecEmail')";

			$dataArray = array(    "ResultCode" => 1,
    		"Message" => "Req sent!",);

		$result3 = mysqli_query($dblink, $query3);
	}
} 
else{
		$dataArray = array(
    	"ResultCode" => 2,
    	"Message" => "User does not exist",);

}

header('Content-Type: application/json');

echo json_encode($dataArray);
?>