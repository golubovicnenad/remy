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

$userEmail = $_POST['userEmail'];

$userEmail = strip_tags($userEmail);

$query1 = "SELECT reciever FROM friendsreq WHERE reciever='$userEmail'";

$result1 = mysqli_query($dblink, $query1);

$row1 = mysqli_fetch_row($result1);

if($row1){
			$dataArray = array(
    	$row1[0]);
	

else{
		$dataArray = array(
    	"ResultCode" => 2,
    	"Message" => "No friend reqs");

}

header('Content-Type: application/json');

echo json_encode($dataArray);
?>