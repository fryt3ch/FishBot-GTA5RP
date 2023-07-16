<html>
<head>
    <title>FishBot update core</title>
</head>
<body>
    <?php
        $current_version = "final_23.8";
    
        if (isset($_POST['check_update']))
            echo "<VERSION>" . $current_version . "</VERSION>";
		else
			echo "Error access!";
    ?>
</body>
</html>