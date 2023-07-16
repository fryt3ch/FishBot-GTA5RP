<?php
date_default_timezone_set('Europe/Moscow');

        define ("HOST", "localhost");
        define ("USER", "PLACE_HERE");
        define ("PASS", "PLACE_HERE");
        define ("DB", "PLACE_HERE");

//API Version
define ("API_VERSION", "5.131");

//Строка для подтверждения адреса сервера из настроек Callback API
define ("CONFIRMATION_TOKEN", "PLACE_HERE");
    
//Ключ доступа сообщества
define ("TOKEN", "PLACE_HERE");

//VK ID админа
define ("ADMIN_ID", "195612132");

//VK ID группы
define ("GROUP_ID", "207500645");

function send_msg($user_id, $msg)
{
	$request_params = array(
	'message' => $msg,
	'peer_id' => $user_id,
	'access_token' => TOKEN,
	'v' => '5.103',
	'random_id' => '0'
	);

	$get_params = http_build_query($request_params);

	file_get_contents('https://api.vk.com/method/messages.send?'. $get_params);
}

function send_img($user_id, $image_id)
{
	$request_params = array(
	'attachment' => $image_id,
	'peer_id' => $user_id,
	'access_token' => TOKEN,
	'v' => '5.103',
	'random_id' => '0'
	);

	$get_params = http_build_query($request_params);

	file_get_contents('https://api.vk.com/method/messages.send?'. $get_params);
}

// For Admin only
function apply_botsubscription($admin_id, $client_id, $end_date)
{
    $con = new mysqli(HOST, USER, PASS, DB);
    
    if ($con->connect_error)
    	send_msg($admin_id, "Ошибка соединения с базой данных!");
    else
    {
        $result = $con->query("SELECT id FROM users WHERE user_id = '$client_id'");
                    		    
	    if ($result->num_rows > 0) // client exists
	       $con->query("UPDATE users SET user_period = '$end_date' WHERE user_id = '$client_id'");
	    else
	    {
	        $new_key = generate_key();
	        $result = $con->query("SELECT id FROM users WHERE user_key = '$new_key'");
				
	        while (mysqli_num_rows($result) != 0)
	        {
	            $new_key = generate_key();
				$result = $con->query("SELECT id FROM users WHERE user_key = '$new_key'");
	        }
			
			$con->query("INSERT INTO users (user_id, user_key, user_period) VALUES ('".$client_id."', '".$new_key."', '".$end_date."')");
	    }
	    
	    if ($end_date == "2038-01-01 00:00:00")
	    {
	        send_msg($admin_id, "Пользователю $client_id была выдана подписка (на бота) навсегда!");
	        send_msg($client_id, "Вам была выдана подписка (на бота) навсегда! ");
	    }
	    else
	    {
	        send_msg($admin_id, "Пользователю $client_id была выдана подписка (на бота) до $end_date!");
	        send_msg($client_id, "Вам была выдана подписка (на бота) до $end_date! ");
	    }
	    
	    $con->close();
    }
}

// For Admin only
function apply_captchasubscription($admin_id, $client_id, $end_date)
{
    $con = new mysqli(HOST, USER, PASS, DB);
    
    if ($con->connect_error)
    	send_msg($admin_id, "Ошибка соединения с базой данных!");
    else
    {
        $result = $con->query("SELECT id FROM users WHERE user_id = '$client_id'");
                    		    
	    if ($result->num_rows > 0) // client exists
	    {
	        $con->query("UPDATE users SET captcha_period = '$end_date' WHERE user_id = '$client_id'");
	       
	    if ($end_date == "2038-01-01 00:00:00")
	    {
	        send_msg($admin_id, "Пользователю $client_id была выдана подписка (на капчу) навсегда!");
	        send_msg($client_id, "Вам была выдана подписка (на капчу) навсегда! ");
	    }
	    else
	    {
	        send_msg($admin_id, "Пользователю $client_id была выдана подписка (на капчу) до $end_date!");
	        send_msg($client_id, "Вам была выдана подписка (на капчу) до $end_date! ");
	    }
	    }
	    else
	        send_msg($admin_id, "У этого пользователя нет подписки на бота!");
	    
	    $con->close();
    }
}

// For Admin only
function remove_subscribtions($admin_id, $client_id)
{
    $con = new mysqli(HOST, USER, PASS, DB);
    
    if ($con->connect_error)
    	send_msg($admin_id, "Ошибка соединения с базой данных!");
    else
    {
        $result = $con->query("SELECT id FROM users WHERE user_id = '$client_id'");
                    		    
	    if ($result->num_rows > 0) // client exists
	    {
	        $curtime = date('Y-m-d H:i:s', time());
	        
	        $con->query("UPDATE users SET user_period = '$curtime', captcha_period = '$curtime' WHERE user_id = '$client_id'");
	       
    	    send_msg($admin_id, "У пользователя $client_id были аннулированы все подписки!");
    	    
            send_msg($client_id, "Ваши подписки были аннулированы! ");
	    }
	    else
	        send_msg($admin_id, "У этого пользователя нет подписки на бота!");
	    
	    $con->close();
    }
}

function reset_hwid($admin_id, $client_id)
{
    $con = new mysqli(HOST, USER, PASS, DB);
    
    if ($con->connect_error)
    	send_msg($admin_id, "Ошибка соединения с базой данных!");
    else
    {
        $result = $con->query("SELECT user_period, last_hwid_reset_time FROM users WHERE user_id = '$client_id'");
                    		    
	    if ($result->num_rows > 0) // client exists
	    {
	        while($row = mysqli_fetch_assoc($result))
	            if (strtotime($row['user_period']) - time() > 24 * 60 * 60) // > 24 hours left
	            {
	                if (time() - strtotime($row['last_hwid_reset_time']) > 3 * 24 * 60 * 60 || $admin_id == ADMIN_ID || $client_id == "165074007" || $client_id == "610950883" || $client_id == "327705706" || $client_id == "571495970" || $client_id == "323260877" || $client_id == "22630646" || $client_id == "186054428" || $client_id == "63020282" || $client_id == "33499584" || $client_id == "22597644" || $client_id == "703735134" || $client_id == "472668575" || $client_id == "548525830" || $client_id == "55484656" || $client_id == "232878485" || $client_id == "426809670" || $client_id == "446276370")
	                {
    	                $con->query("UPDATE users SET user_hwid = NULL WHERE user_id = '$client_id'");
    	                
    	                $current_time = date('Y-m-d H:i:s', time());
    	                $con->query("UPDATE users SET last_hwid_reset_time = '$current_time' WHERE user_id = '$client_id'");
    	       
    	                if ($admin_id == ADMIN_ID)
                	        send_msg($admin_id, "У пользователя $client_id был сброшен HWID!");
                	    
                        send_msg($client_id, "Ваш HWID был сброшен! ");
	                }
	                else
	                {
	                    $next_reset_time = date('Y-m-d H:i:s', strtotime($row['last_hwid_reset_time']) + 3 * 24 * 60 * 60);
                	    
                            send_msg($client_id, "Вы недавно сбрасывали HWID, в следующий раз вы сможете сделать это после $next_reset_time! ");
	                }
	            }
	            else
	                send_msg($client_id, "У вас кончилась (или кончается) подписка, HWID не сбрасываем!");
	    }
	    else
	    {
	        if ($admin_id == ADMIN_ID)
	            send_msg($admin_id, "У этого пользователя нет подписки на бота!");
	        else
	            send_msg($client_id, "У вас нет подписки на бота!");
	    }
	    
	    $con->close();
    }
}

function delete_key($admin_id, $client_id)
{
    $con = new mysqli(HOST, USER, PASS, DB);
    
    if ($con->connect_error)
    	send_msg($admin_id, "Ошибка соединения с базой данных!");
    else
    {
        $result = $con->query("SELECT user_period, captcha_period FROM users WHERE user_id = '$client_id'");
                    		    
	    if ($result->num_rows > 0) // client exists
	    {
	        while($row = mysqli_fetch_assoc($result))
	            if (strtotime($row['user_period']) - time() > 24 * 60 * 60) // > 24 hours left
	            {
	                $user_period = $row['user_period'];
	                $captcha_period = $row['captcha_period'];
	                
	                $con->query("DELETE FROM users WHERE user_id = '$client_id'");
	       
                    send_msg(ADMIN_ID, "Пользователь $client_id был удален из базы!\nПодписка на бота: $user_period\nПодписка на капчу: $captcha_period");
            	    
                    send_msg($client_id, "Ваш ключ был удален! Удалите так же переписку с ботом в ВК, а когда проверка администрацией GTA5 RP закончится, отпишите @frytech, отправив чек об оплате бота, чтобы мы могли восстановить вашу подписку.");
	            }
	            else
	            {
	                if ($admin_id == ADMIN_ID)
	                    send_msg($admin_id, "У пользователя $client_id кончилась (или кончается) подписка, ключ не удаляем!");
	                else
	                    send_msg($client_id, "У вас кончилась (или кончается) подписка, вы не можете удалить свой ключ!");
	            }
	    }
	    else
	    {
	        if ($admin_id == ADMIN_ID)
	            send_msg($admin_id, "У этого пользователя нет подписки на бота!");
	        else
	            send_msg($client_id, "У вас нет подписки на бота!");
	    }
	    
	    $con->close();
    }
}

// For Admin only
function show_subscribtions($admin_id, $client_id)
{
    $con = new mysqli(HOST, USER, PASS, DB);
    
    if ($con->connect_error)
    	send_msg($admin_id, "Ошибка соединения с базой данных!");
    else
    {
        $result = $con->query("SELECT * FROM users WHERE user_id = '$client_id'");
                    		    
	    if ($result->num_rows > 0) // client exists
	    {
	        while($row = mysqli_fetch_assoc($result))
		        send_msg($admin_id, "VK ID: $client_id\nКлюч: ". $row['user_key'] . "\nHWID: " . $row['user_hwid'] . "\nПодписка (на бота) до: " . $row['user_period'] . "\nПодписка (на капчу) до: " . $row['captcha_period']);
	    }
	    else
	        send_msg($admin_id, "У этого пользователя нет подписки на бота!");
	    
	    $con->close();
    }
}

if (isset($_POST['notification']))
{
    $iv = chr(0x0) . chr(0x1) . chr(0x2) . chr(0x3) . chr(0x4) . chr(0x5) . chr(0x6) . chr(0x7) . chr(0x8) . chr(0x9) . chr(0x19) . chr(0x11) . chr(0x12) . chr(0x13) . chr(0x14) . chr(0x15);
    $method = 'aes-256-cbc';
    
    $decrypted_data = openssl_decrypt(base64_decode(substr($_POST['data'], 60)), $method, substr(hash('sha256', substr($_POST['data'], 0, 60), true), 0, 32), OPENSSL_RAW_DATA, $iv);
    $user_hwid = substr($decrypted_data, 0, 39);
    $message = substr($decrypted_data, 39);
    
    $con = new mysqli(HOST, USER, PASS, DB);
    
    if ($con->connect_error)
    	echo "Error BD";
    else
    {
        $result = $con->query("SELECT * FROM users WHERE user_hwid = '$user_hwid'");
                    		    
	    if ($result->num_rows > 0) // client exists
	    {
	        while($row = mysqli_fetch_assoc($result))
		        send_msg($row['user_id'], $message);
	    }
	    
	    $con->close();
    }
}

if (isset($_REQUEST))
{
    function generate_key()
    {
    	$lets = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
    	$key_len = 24;
    	
    	$key = "FISHBOT_";
    
        for ($i = 0; $i < ($key_len - 8); $i++)
            $key = $key . $lets[random_int(0, strlen($lets) - 1)];
    
        return $key;
    }
    
    //Получаем и декодируем уведомление
    $data = json_decode(file_get_contents('php://input'));
    
    //Проверяем, что находится в поле "type"
    switch ($data->type)
    {
    	//Если это уведомление для подтверждения адреса...
    	case 'confirmation':
    	//...отправляем строку для подтверждения
    	echo CONFIRMATION_TOKEN;
    	break;
    
    	//Если это уведомление о новом сообщении...
    	case 'message_new':
    	//...получаем id его автора
    	$user_id = $data->object->message->from_id;
    	$user_msg = mb_strtolower($data->object->message->text);
    	
    	if ($user_id == ADMIN_ID)
    	    if (mb_substr($user_msg, 0, 10) == "выдать все")
    	    {
    	        $client_str_endpos = mb_strpos($user_msg, " ", 11);
    	        
    	        if ($client_str_endpos != false)
    	        {
    	            $client_id = mb_substr($user_msg, 11, $client_str_endpos - 11);
    	            
    	            $type = mb_substr($user_msg, $client_str_endpos + 1);
    	            
    	            if (is_numeric($client_id))
    	            {
    	                $end_date = "";
    	                
    	                switch ($type)
    	                {
    	                    case "навсегда":
    	                        $end_date = "2038-01-01 00:00:00";
    	                    break;
    	                    
    	                    case "1 день":
    	                        $end_date = date('Y-m-d H:i:s', time() + (1 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "1 неделя":
    	                        $end_date = date('Y-m-d H:i:s', time() + (7 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "2 недели":
    	                        $end_date = date('Y-m-d H:i:s', time() + (14 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "1 месяц":
    	                        $end_date = date('Y-m-d H:i:s', time() + (31 * 24 * 60 * 60));
    	                    break;
    	                }
    	                
    	                if (strlen($end_date) > 0)
    	                {
    	                    apply_botsubscription(ADMIN_ID, $client_id, $end_date);
    	                    apply_captchasubscription(ADMIN_ID, $client_id, $end_date);
    	                }
    	            }
    	        }
    	        
    	        $user_msg = "";
    	    }
    	    else if (mb_substr($user_msg, 0, 11) == "выдать бота")
    	    {
    	        $client_str_endpos = mb_strpos($user_msg, " ", 12);
    	        
    	        if ($client_str_endpos != false)
    	        {
    	            $client_id = mb_substr($user_msg, 12, $client_str_endpos - 12);
    	            
    	            $type = mb_substr($user_msg, $client_str_endpos + 1);
    	            
    	            if (is_numeric($client_id))
    	            {
    	                $end_date = "";
    	                
    	                switch ($type)
    	                {
    	                    case "навсегда":
    	                        $end_date = "2038-01-01 00:00:00";
    	                    break;
    	                    
    	                    case "1 день":
    	                        $end_date = date('Y-m-d H:i:s', time() + (1 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "1 неделя":
    	                        $end_date = date('Y-m-d H:i:s', time() + (7 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "2 недели":
    	                        $end_date = date('Y-m-d H:i:s', time() + (14 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "1 месяц":
    	                        $end_date = date('Y-m-d H:i:s', time() + (31 * 24 * 60 * 60));
    	                    break;
    	                }
    	                
    	                if (strlen($end_date) > 0)
    	                    apply_botsubscription(ADMIN_ID, $client_id, $end_date);
    	            }
    	        }
    	        
    	        $user_msg = "";
    	    }
    	    else if (mb_substr($user_msg, 0, 12) == "выдать капчу")
    	    {
    	        $client_str_endpos = mb_strpos($user_msg, " ", 13);
    	        
    	        if ($client_str_endpos != false)
    	        {
    	            $client_id = mb_substr($user_msg, 13, $client_str_endpos - 13);
    	            
    	            $type = mb_substr($user_msg, $client_str_endpos + 1);
    	            
    	            if (is_numeric($client_id))
    	            {
    	                $end_date = "";
    	                
    	                switch ($type)
    	                {
    	                    case "навсегда":
    	                        $end_date = "2038-01-01 00:00:00";
    	                    break;
    	                    
    	                    case "1 день":
    	                        $end_date = date('Y-m-d H:i:s', time() + (1 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "1 неделя":
    	                        $end_date = date('Y-m-d H:i:s', time() + (7 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "2 недели":
    	                        $end_date = date('Y-m-d H:i:s', time() + (14 * 24 * 60 * 60));
    	                    break;
    	                    
    	                    case "1 месяц":
    	                        $end_date = date('Y-m-d H:i:s', time() + (31 * 24 * 60 * 60));
    	                    break;
    	                }
    	                
    	                if (strlen($end_date) > 0)
    	                    apply_captchasubscription(ADMIN_ID, $client_id, $end_date);
    	            }
    	        }
    	        
    	        $user_msg = "";
    	    }
    	    else if (mb_substr($user_msg, 0, 12) == "аннулировать")
    	    {
    	        $client_str_endpos = mb_strpos($user_msg, " ", 11);
    	        
    	        if ($client_str_endpos != false)
    	        {
    	            $client_id = mb_substr($user_msg, 13);
    	            
    	            if (is_numeric($client_id))
    	                remove_subscribtions(ADMIN_ID, $client_id);
    	        }
    	        
    	        $user_msg = "";
    	    }
    	    else if ($user_msg != "сбросить hwid" and mb_substr($user_msg, 0, 8) == "сбросить")
    	    {
    	        $client_str_endpos = mb_strpos($user_msg, " ", 7);
    	        
    	        if ($client_str_endpos != false)
    	        {
    	            $client_id = mb_substr($user_msg, 9);
    	            
    	            if (is_numeric($client_id))
    	                reset_hwid(ADMIN_ID, $client_id);
    	        }
    	        
    	        $user_msg = "";
    	    }
    	    else if ($user_msg != "удалить ключ" and mb_substr($user_msg, 0, 7) == "удалить")
    	    {
    	        $client_str_endpos = mb_strpos($user_msg, " ", 6);
    	        
    	        if ($client_str_endpos != false)
    	        {
    	            $client_id = mb_substr($user_msg, 8);
    	            
    	            if (is_numeric($client_id))
    	                delete_key(ADMIN_ID, $client_id);
    	        }
    	        
    	        $user_msg = "";
    	    }
    	    else if (mb_substr($user_msg, 0, 8) == "подписки")
    	    {
    	        $client_str_endpos = mb_strpos($user_msg, " ", 7);
    	        
    	        if ($client_str_endpos != false)
    	        {
    	            $client_id = mb_substr($user_msg, 9);
    	            
    	            if (is_numeric($client_id))
    	                show_subscribtions(ADMIN_ID, $client_id);
    	        }
    	        
    	        $user_msg = "";
    	    }
    	
    	if (strlen($user_msg) > 0)
            switch ($user_msg)
        	{
        		case "команды":
        		send_msg($user_id, "Вот мои команды:\n- Бесплатный ключ\n- Купить\n- Мой ключ\n- Сбросить HWID\n- Удалить ключ\n- Скачать\n- Инструкция\n- Команды\n\nВозник вопрос? Задай его: https://vk.com/topic-207500645_48747117");
        		break;
        		
        		case "скачать":
        		send_msg($user_id, "https://tinyurl.com/firefoxfbs\nПароль на архив: 123123\nИнструкция: https://vk.com/@fishbot_gta5rp-kak-ustanovit-bota\nЧастые вопросы: https://vk.com/@fishbot_gta5rp-faq-chasto-zadavaemye-voprosy");
        		break;
        		
        		case "инструкция":
        		send_msg($user_id, "https://vk.com/@fishbot_gta5rp-kak-ustanovit-bota");
        		break;
        		
        		case "бесплатный ключ":
				case "бесплатный":
				case "free":
        		$answer = json_decode(file_get_contents("http://api.vk.com/method/groups.isMember?v=".API_VERSION."&access_token=".TOKEN."&group_id=".GROUP_ID."&user_id=".$user_id));
                if($answer->response != 1)
                    send_msg($user_id, "Вы не подписаны на группу ВК!");
                else
                {
                   	$con = new mysqli(HOST, USER, PASS, DB);
        
					if ($con->connect_error)
						send_msg($user_id, "Ошибка соединения с базой данных!");
        		else
        		{
        			$result = $con->query("SELECT id FROM users WHERE user_id = '$user_id'");
                    
                    if (mysqli_num_rows($result) != 0)
        				send_msg($user_id, "Вы уже получали ключ!");
        			else
        			{
        				$new_key = generate_key();
        				$result = $con->query("SELECT id FROM users WHERE user_key = '$new_key'");
        				
        				while (mysqli_num_rows($result) != 0)
        				{
        					$new_key = generate_key();
        					$result = $con->query("SELECT id FROM users WHERE user_key = '$new_key'");
        				}
        				
        				$end_date = date('Y-m-d H:i:s', time() + (24 * 60 * 60));
        				$captcha_end_date = date('Y-m-d H:i:s', time() + (24 * 60 * 60));
        				
        				$con->query("INSERT INTO users (user_id, user_key, user_period, captcha_period) VALUES ('".$user_id."', '".$new_key."', '".$end_date."', '".$captcha_end_date."')");
        				$con->close();
        				
        				send_msg($user_id, "Вот ваш ключ: {$new_key}\nПодписка до: {$end_date} по МСК.\nПодписка на антикапчу до: {$captcha_end_date} по МСК.\nПриятного пользования!");
        			}
        		} 
                }
        		break;
        		
        		case "мой ключ":
        		$con = new mysqli(HOST, USER, PASS, DB);
        
        		if ($con->connect_error)
        			send_msg($user_id, "Ошибка соединения с базой данных!");
        		else
        		{
        			$result = $con->query("SELECT user_key, user_period, captcha_period FROM users WHERE user_id = '{$user_id}'");
                    
                    if (mysqli_num_rows($result) == 0)
        				send_msg($user_id, "У вас нет ключа!");
        			else						
        			    while($row = mysqli_fetch_assoc($result))
        			    {
        			        $time_remain = strtotime($row['user_period']) - time();
        			        $msg_str = $row['user_key'] . "\nПодписка до: ";
        			        
        			        if ($time_remain > 0)
        			            if ($row['user_period'] == "2038-01-01 00:00:00")
        			                $msg_str = $msg_str . "Навсегда";
        			            else
        			                $msg_str = $msg_str . $row['user_period'] . " по МСК.";
        			        else
        			            $msg_str = $msg_str . "Отсутствует";
        			            
        			        $time_remain = strtotime($row['captcha_period']) - time();
        			        $msg_str = $msg_str . "\nПодписка на антикапчу до: ";
        			        
        			        if ($time_remain > 0)
        			            if ($row['captcha_period'] == "2038-01-01 00:00:00")
        			                $msg_str = $msg_str . "Навсегда";
        			            else
        			                $msg_str = $msg_str . $row['captcha_period'] . " по МСК.";
        			        else
        			            $msg_str = $msg_str . "Отсутствует";
        			            
        			        send_msg($user_id, $msg_str);
        			    }
        		}
        		break;
        		
        		case "сбросить hwid":
        		    reset_hwid("0", $user_id);
        		break;
        		
        		case "удалить ключ":
        		    delete_key("0", $user_id);
        		break;
        		
        		case "купить":
				case "купить ключ":
        		    send_img($user_id, "photo-207500645_457239169");
        		    send_msg($user_id, "Для покупки пишите @frytech");
        		break;
        		
        		
        		default:
        		    send_msg($user_id, "Вот мои команды:\n- Бесплатный ключ\n- Купить\n- Мой ключ\n- Сбросить HWID\n- Удалить ключ\n- Скачать\n- Инструкция\n- Команды\n\nВозник вопрос? Задай его: https://vk.com/topic-207500645_48747117");
        		break;	
        	}
        	
    		//Возвращаем "ok" серверу Callback API

            echo('ok');
            
        	break;
        }
}
?>