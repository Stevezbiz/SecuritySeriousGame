<?php

    header('Access-Control-Allow-Origin: *');
    header("Access-Control-Allow-Credentials: true");
    header('Access-Control-Allow-Methods: GET, PUT, POST, DELETE, OPTIONS');
    header('Access-Control-Max-Age: 1000');
    header('Access-Control-Allow-Headers: Content-Type, Content-Range, Content-Disposition, Content-Description');
?>
<?php

	error_reporting(0);
	ini_set('display_errors', 0);
	
	
	$mode = $_POST["mode"];

	$playersFile = $_POST["playersFile"];
	
	$playersFilePath = '../'.$playersFile;	
	
	//read file content and send to unity
	if($mode == 'r')
	{
		if(!file_exists($playersFilePath))
		{
			file_put_contents($playersFilePath, "");
			echo "File Created";
		}
		else if(file_get_contents($playersFilePath)==FALSE)
		{
			echo "File Empty";
		}
		else
		{
			$file = file_get_contents($playersFilePath);
			echo $file;		
		}	
	}	
	//write settings value into file
	else
	{
		$playerList = $_POST["playerList"];
		
		if(file_put_contents($playersFilePath, $playerList) !== FALSE)
		{
			echo "File Updated";
		}
		else
		{
			echo "Error Updating File";		
		}
	}

?>
