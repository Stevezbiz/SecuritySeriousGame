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
	
	$saveFile = $_POST["saveFile"];

	$playerSaveFilePath = '../'.$saveFile;
	
	
	//read file content and send to unity
	if($mode == 'r')
	{
		if(!file_exists($playerSaveFilePath))
		{
			echo "Save Not Present";
		}
		else if(file_get_contents($playerSaveFilePath) !== FALSE)
		{
			$file = file_get_contents($playerSaveFilePath);
			echo $file;
		}
		else
		{			
			echo "Error Loading File";
		}
	}
	//write gamedata value into file
	else if ($mode == 'w')
	{
		$saveContent = $_POST["gameSave"];
		if (file_put_contents($playerSaveFilePath, $saveContent) !== FALSE) 
		{
			echo "File updated";		
		} 
		else 
		{
			echo "Error Saving File";
		}
	}
	//delete gamedata file from server
	else
	{		
		if(file_get_contents($playerSaveFilePath) !== FALSE)
		{
			unlink(realpath($playerSaveFilePath));
			echo "File Deleted";
		}
		else
		{			
			echo "Error Deleting File";
		}
	}

?>