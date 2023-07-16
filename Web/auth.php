<html>
<head>
    <title>FishBot authorization core</title>
</head>
<body>
    <?php
        $timezone = 'Europe/Moscow';
        date_default_timezone_set($timezone);
        
        define ("HOST", "localhost");
        define ("USER", "PLACE_HERE");
        define ("PASS", "PLACE_HERE");
        define ("DB", "PLACE_HERE");
    
        $XML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>
        <fishbot>
              <binarization captcha=\"65\" items=\"85\" weight=\"120\" fish=\"150\">
              </binarization>
              <invbox_side value=\"90\">
              </invbox_side>
          <resolutions>
		      <resolution value=\"3840x2160\">
              <rect value=\"smiles\" X=\"625\" Y=\"2015\" width=\"165\" height=\"45\">
              </rect>
              <rect value=\"mouse\" X=\"2005\" Y=\"1755\" width=\"15\" height=\"20\">
              </rect>
              <rect value=\"result\" X=\"1600\" Y=\"2095\" width=\"35\" height=\"40\">
              </rect>
              <rect value=\"fish\" X=\"1780\" Y=\"2100\" width=\"270\" height=\"35\">
              </rect>
              <rect value=\"captcha_text\" X=\"970\" Y=\"615\" width=\"140\" height=\"35\">
              </rect>
              <rect value=\"captcha\" X=\"810\" Y=\"435\" width=\"295\" height=\"95\">
              </rect>
              <rect value=\"inventory\" X=\"3372\" Y=\"880\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_weight\" X=\"3550\" Y=\"845\" width=\"70\" height=\"30\">
              </rect>
              <rect value=\"inventory_wbag\" X=\"3372\" Y=\"765\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_wbag_weight\" X=\"3550\" Y=\"730\" width=\"70\" height=\"30\">
              </rect>
              <rect value=\"bag\" X=\"3372\" Y=\"1175\" width=\"446\" height=\"176\">
              </rect>
              <rect value=\"boat_inv\" X=\"1351\" Y=\"896\" width=\"525\" height=\"345\">
              </rect>
              <point value=\"drop_max\" X=\"2035\" Y=\"1120\">
              </point>
              <point value=\"drop_apply\" X=\"1825\" Y=\"1200\">
              </point>
              <point value=\"captcha_start\" X=\"1795\" Y=\"1025\">
              </point>
              <point value=\"boat_close\" X=\"1915\" Y=\"1355\">
              </point>
			  <point value=\"captcha_apply\" X=\"560\" Y=\"600\">
              </point>
            </resolution>
			<resolution value=\"2560x1440\">
              <rect value=\"smiles\" X=\"425\" Y=\"1300\" width=\"165\" height=\"45\">
              </rect>
              <rect value=\"mouse\" X=\"1370\" Y=\"1185\" width=\"10\" height=\"20\">
              </rect>
              <rect value=\"result\" X=\"955\" Y=\"1375\" width=\"35\" height=\"40\">
              </rect>
              <rect value=\"fish\" X=\"1140\" Y=\"1380\" width=\"270\" height=\"35\">
              </rect>
              <rect value=\"captcha_text\" X=\"1290\" Y=\"765\" width=\"140\" height=\"35\">
              </rect>
              <rect value=\"captcha\" X=\"1130\" Y=\"625\" width=\"295\" height=\"95\">
              </rect>
              <rect value=\"inventory\" X=\"2092\" Y=\"520\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_weight\" X=\"2265\" Y=\"485\" width=\"75\" height=\"30\">
              </rect>
              <rect value=\"inventory_wbag\" X=\"2092\" Y=\"406\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_wbag_weight\" X=\"2265\" Y=\"374\" width=\"75\" height=\"30\">
              </rect>
              <rect value=\"bag\" X=\"2092\" Y=\"815\" width=\"446\" height=\"176\">
              </rect>
              <rect value=\"boat_inv\" X=\"711\" Y=\"536\" width=\"525\" height=\"345\">
              </rect>
              <point value=\"drop_max\" X=\"1420\" Y=\"755\">
              </point>
              <point value=\"drop_apply\" X=\"1185\" Y=\"840\">
              </point>
              <point value=\"captcha_start\" X=\"1275\" Y=\"725\">
              </point>
              <point value=\"boat_close\" X=\"1275\" Y=\"995\">
              </point>
			  <point value=\"captcha_apply\" X=\"1200\" Y=\"780\">
              </point>
            </resolution>
            <resolution value=\"1920x1080\">
              <rect value=\"smiles\" X=\"323\" Y=\"950\" width=\"165\" height=\"45\">
              </rect>
              <rect value=\"mouse\" X=\"1040\" Y=\"895\" width=\"35\" height=\"35\">
              </rect>
              <rect value=\"result\" X=\"635\" Y=\"1015\" width=\"45\" height=\"45\">
              </rect>
              <rect value=\"fish\" X=\"830\" Y=\"1015\" width=\"270\" height=\"45\">
              </rect>
              <rect value=\"captcha_text\" X=\"970\" Y=\"585\" width=\"140\" height=\"35\">
              </rect>
              <rect value=\"captcha\" X=\"810\" Y=\"435\" width=\"295\" height=\"95\">
              </rect>
              <rect value=\"inventory\" X=\"1452\" Y=\"340\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_weight\" X=\"1630\" Y=\"300\" width=\"70\" height=\"30\">
              </rect>
              <rect value=\"inventory_wbag\" X=\"1452\" Y=\"225\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_wbag_weight\" X=\"1630\" Y=\"185\" width=\"70\" height=\"30\">
              </rect>
              <rect value=\"bag\" X=\"1452\" Y=\"635\" width=\"446\" height=\"176\">
              </rect>
              <rect value=\"boat_inv\" X=\"390\" Y=\"360\" width=\"525\" height=\"345\">
              </rect>
              <point value=\"drop_max\" X=\"1100\" Y=\"575\">
              </point>
              <point value=\"drop_apply\" X=\"860\" Y=\"660\">
              </point>
              <point value=\"captcha_start\" X=\"965\" Y=\"550\">
              </point>
              <point value=\"boat_close\" X=\"950\" Y=\"815\">
              </point>
			  <point value=\"captcha_apply\" X=\"880\" Y=\"605\">
              </point>
            </resolution>
            <resolution value=\"1600x900\">
              <rect value=\"smiles\" X=\"275\" Y=\"780\" width=\"140\" height=\"40\">
              </rect>
              <rect value=\"mouse\" X=\"885\" Y=\"745\" width=\"15\" height=\"30\">
              </rect>
              <rect value=\"result\" X=\"480\" Y=\"835\" width=\"35\" height=\"40\">
              </rect>
              <rect value=\"fish\" X=\"670\" Y=\"840\" width=\"270\" height=\"30\">
              </rect>
              <rect value=\"captcha_text\" X=\"810\" Y=\"490\" width=\"140\" height=\"35\">
              </rect>
              <rect value=\"captcha\" X=\"650\" Y=\"315\" width=\"295\" height=\"95\">
              </rect>
              <rect value=\"inventory\" X=\"1132\" Y=\"250\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_weight\" X=\"1310\" Y=\"215\" width=\"70\" height=\"25\">
              </rect>
              <rect value=\"inventory_wbag\" X=\"1132\" Y=\"135\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_wbag_weight\" X=\"1310\" Y=\"100\" width=\"70\" height=\"30\">
              </rect>
              <rect value=\"bag\" X=\"1132\" Y=\"545\" width=\"446\" height=\"176\">
              </rect>
              <rect value=\"boat_inv\" X=\"230\" Y=\"270\" width=\"525\" height=\"345\">
              </rect>
              <point value=\"drop_max\" X=\"940\" Y=\"485\">
              </point>
              <point value=\"drop_apply\" X=\"700\" Y=\"565\">
              </point>
              <point value=\"captcha_start\" X=\"800\" Y=\"460\">
              </point>
              <point value=\"boat_close\" X=\"800\" Y=\"725\">
              </point>
			  <point value=\"captcha_apply\" X=\"700\" Y=\"510\">
              </point>
            </resolution>
            <resolution value=\"1366x768\">
              <rect value=\"smiles\" X=\"245\" Y=\"650\" width=\"160\" height=\"40\">
              </rect>
              <rect value=\"mouse\" X=\"765\" Y=\"645\" width=\"20\" height=\"25\">
              </rect>
              <rect value=\"result\" X=\"360\" Y=\"705\" width=\"40\" height=\"40\">
              </rect>
              <rect value=\"fish\" X=\"545\" Y=\"705\" width=\"270\" height=\"40\">
              </rect>
              <rect value=\"captcha_text\" X=\"690\" Y=\"425\" width=\"140\" height=\"35\">
              </rect>
              <rect value=\"captcha\" X=\"535\" Y=\"275\" width=\"265\" height=\"95\">
              </rect>
              <rect value=\"inventory\" X=\"899\" Y=\"184\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_weight\" X=\"1055\" Y=\"145\" width=\"90\" height=\"30\">
              </rect>
              <rect value=\"inventory_wbag\" X=\"899\" Y=\"69\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_wbag_weight\" X=\"1055\" Y=\"30\" width=\"90\" height=\"30\">
              </rect>
              <rect value=\"bag\" X=\"899\" Y=\"479\" width=\"446\" height=\"176\">
              </rect>
              <rect value=\"boat_inv\" X=\"112\" Y=\"200\" width=\"525\" height=\"345\">
              </rect>
              <point value=\"drop_max\" X=\"790\" Y=\"420\">
              </point>
              <point value=\"drop_apply\" X=\"585\" Y=\"500\">
              </point>
              <point value=\"captcha_start\" X=\"680\" Y=\"390\">
              </point>
              <point value=\"boat_close\" X=\"700\" Y=\"660\">
              </point>
			  <point value=\"captcha_apply\" X=\"600\" Y=\"450\">
              </point>
            </resolution>
            <resolution value=\"1280x1024\">
              <rect value=\"smiles\" X=\"300\" Y=\"900\" width=\"155\" height=\"40\">
              </rect>
              <rect value=\"mouse\" X=\"720\" Y=\"845\" width=\"20\" height=\"30\">
              </rect>
              <rect value=\"result\" X=\"315\" Y=\"955\" width=\"40\" height=\"45\">
              </rect>
              <rect value=\"fish\" X=\"500\" Y=\"965\" width=\"270\" height=\"30\">
              </rect>
              <rect value=\"captcha_text\" X=\"650\" Y=\"555\" width=\"140\" height=\"35\">
              </rect>
              <rect value=\"captcha\" X=\"490\" Y=\"375\" width=\"295\" height=\"95\">
              </rect>
              <rect value=\"inventory\" X=\"812\" Y=\"312\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_weight\" X=\"980\" Y=\"275\" width=\"80\" height=\"25\">
              </rect>
              <rect value=\"inventory_wbag\" X=\"812\" Y=\"197\" width=\"446\" height=\"356\">
              </rect>
              <rect value=\"inv_wbag_weight\" X=\"980\" Y=\"160\" width=\"80\" height=\"25\">
              </rect>
              <rect value=\"bag\" X=\"812\" Y=\"722\" width=\"446\" height=\"176\">
              </rect>
              <rect value=\"boat_inv\" X=\"70\" Y=\"330\" width=\"525\" height=\"345\">
              </rect>
              <point value=\"drop_max\" X=\"755\" Y=\"545\">
              </point>
              <point value=\"drop_apply\" X=\"540\" Y=\"630\">
              </point>
              <point value=\"captcha_start\" X=\"640\" Y=\"520\">
              </point>
              <point value=\"boat_close\" X=\"650\" Y=\"785\">
              </point>
			  <point value=\"captcha_apply\" X=\"560\" Y=\"570\">
              </point>
            </resolution>
          </resolutions>
        </fishbot>";
    
    if (isset($_POST['menu_auth']))
    {   
        $iv = chr(0x0) . chr(0x1) . chr(0x2) . chr(0x3) . chr(0x4) . chr(0x5) . chr(0x6) . chr(0x7) . chr(0x8) . chr(0x9) . chr(0x19) . chr(0x11) . chr(0x12) . chr(0x13) . chr(0x14) . chr(0x15);
        $method = 'aes-256-cbc';
        
        $decrypted_data = openssl_decrypt(base64_decode(substr($_POST['data'], 44)), $method, substr(hash('sha256', base64_decode(substr($_POST['data'], 0, 44)), true), 0, 32), OPENSSL_RAW_DATA, $iv);
        
        $user_key = substr($decrypted_data, 0, 24);
        $user_hwid = substr($decrypted_data, 24);
        
        // Length of md5 hash is always 32, length of base64 encoded string (length 32) is always 44
        // We remove last symbol (=) to confuse reverser (he wouldn't know it's base64 hash with constant length)
        $token = substr(base64_encode(md5(date('Y-m-d H:i:s', time()) . $user_hwid . $user_key)), 0, -1);
        
        $password = substr(hash('sha256', $token, true), 0, 32);
        
        if (strlen($user_hwid) != 39 || strlen($user_key) != 24)
        {
            $encrypted_data = openssl_encrypt("HWID ERROR", $method, $password, $options=0, $iv);
        
            echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";
        }
        else
        {
           $con = new mysqli(HOST, USER, PASS, DB);
        
            if ($con->connect_error)
            {
                $encrypted_data = openssl_encrypt("Ошибка подключения к базе данных!", $method, $password, $options=0, $iv);
            
                echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";
            }
            else
            {
                $result = $con->query("SELECT * FROM users WHERE user_key = '$user_key'");
                
                if (mysqli_num_rows($result) == 0)
                {
                    $encrypted_data = openssl_encrypt("Неверный ключ!", $method, $password, $options=0, $iv);
            
                    echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";  
                }
                else
                {
                    while ($row = $result->fetch_assoc())
                    {
                        // New User
                        if (is_null($row['user_hwid']))
                        {
                            $res = $con->query("SELECT id FROM users WHERE user_hwid = '$user_hwid'");
                            
                            if (mysqli_num_rows($res) != 0)
                            {
                                $encrypted_data = openssl_encrypt("У вас уже есть ключ!\nПросто купи ключ, поддержи чужой труд :)", $method, $password, $options=0, $iv);
                            
                                echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";
                            }
                            else
                            {
                                // Valid HWID
                                $con->query("UPDATE users SET user_hwid = '$user_hwid' WHERE user_key = '$user_key'");
                                
                                $bot_period = DateTime::createFromFormat(
                                                'Y-m-d H:i:s',
                                                $row['user_period'],
                                                new DateTimeZone($timezone)
                                            );
                                            
                                $captcha_period = DateTime::createFromFormat(
                                                'Y-m-d H:i:s',
                                                $row['captcha_period'],
                                                new DateTimeZone($timezone)
                                            );
                                            
                                $bot_remain = ($bot_period->getTimestamp() - time());
                                $captcha_remain = ($captcha_period->getTimestamp() - time());
                                
                                if ($captcha_remain < 0)
                                    $captcha_remain = 0;
                                
                                if ($bot_remain > 30)
                                {
                                    $data = ("<TIMESTAMP>" . date('Y-m-d H:i:s', time()) . "</TIMESTAMP>");
                                    $data .= ("<BOT_SECS>" . $bot_remain . "</BOT_SECS>");
                                    $data .= ("<CAPTCHA_SECS>" . $captcha_remain . "</CAPTCHA_SECS>");
                                    
                                    if ($_POST['xml_required'] == "true")
                                        $data .= ("<XML>" . $XML . "</XML>");
                                    
                                    $encrypted_data = openssl_encrypt($data, $method, $password, $options=0, $iv);
                                    
                                    echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";
                                }
                                else
                                {
                                    $encrypted_data = openssl_encrypt("Ваша подписка кончилась!", $method, $password, $options=0, $iv);
                                
                                    echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";
                                }
                            }
                        }
                        else // Old User
                        {
                            // Valid HWID
                            if ($user_hwid == $row['user_hwid'])
                            {
                                $bot_period = DateTime::createFromFormat(
                                                'Y-m-d H:i:s',
                                                $row['user_period'],
                                                new DateTimeZone($timezone)
                                            );
                                            
                                $captcha_period = DateTime::createFromFormat(
                                                'Y-m-d H:i:s',
                                                $row['captcha_period'],
                                                new DateTimeZone($timezone)
                                            );
                                            
                                $bot_remain = ($bot_period->getTimestamp() - time());
                                $captcha_remain = ($captcha_period->getTimestamp() - time());
                                
                                if ($captcha_remain < 0)
                                    $captcha_remain = 0;
                                
                                if ($bot_remain > 30)
                                {
                                    $data = ("<TIMESTAMP>" . date('Y-m-d H:i:s', time()) . "</TIMESTAMP>");
                                    $data .= ("<BOT_SECS>" . $bot_remain . "</BOT_SECS>");
                                    $data .= ("<CAPTCHA_SECS>" . $captcha_remain . "</CAPTCHA_SECS>");
                                    
                                    if ($_POST['xml_required'] == "true")
                                        $data .= ("<XML>" . $XML . "</XML>");
                                    
                                    $encrypted_data = openssl_encrypt($data, $method, $password, $options=0, $iv);
                                    
                                    echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";
                                }
                                else
                                {
                                    $encrypted_data = openssl_encrypt("Ваша подписка кончилась!\nНо это - не проблема, у нас в группе частые розыгрыши, а если не хочешь ждать, то у нас самые низкие цены!", $method, $password, $options=0, $iv);
                                    
                                    echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";
                                }
                            }
                            else
                            {
                                $encrypted_data = openssl_encrypt("Этот ключ не ваш!\nЕсли это ошибка, сбросьте HWID, написав боту в группе ВК", $method, $password, $options=0, $iv);
                                
                                echo "<RESULT>" . $token . $encrypted_data . "</RESULT>";
                            }
                        }
                    }
                }
            } 
        }
    }
    else if (isset($_POST['anticaptcha_auth']))
    {
        $user_hwid = $_POST['user_hwid'];
        
        $con = new mysqli(HOST, USER, PASS, DB);
        
        if ($con->connect_error)
            // Error BD
            echo "<RESULT>" . "ERROR BD" . "</RESULT>";
        else
        {
            $result = $con->query("SELECT user_key, captcha_period FROM users WHERE user_hwid = '$user_hwid'");
            
            if (mysqli_num_rows($result) == 0)
                // Incorrect Key (Неверный ключ!)
                echo "<RESULT>" . "NO USER" . "</RESULT>";
            else
            {
                while ($row = $result->fetch_assoc())
                {
                    $captcha_period = DateTime::createFromFormat(
                        'Y-m-d H:i:s',
                        $row['captcha_period'],
                        new DateTimeZone($timezone)
                    );
                                        
                    echo "<RESULT>" . "OK" . "</RESULT>";
                    echo "<TIME_REMAIN>" . ($captcha_period->getTimestamp() - time()) . "</TIME_REMAIN>";
                    echo "<USER_KEY>" . $row['user_key'] . "</USER_KEY>";
                }
            }
        }
    }
    else echo "Error access!";
    ?>
</body>
</html>