<?php
    //file_put_contents("input.log", file_get_contents('php://input'));
    //file_put_contents("globals.log", print_r($GLOBALS,true));

    if ( $_SERVER['REQUEST_METHOD']=='GET' && realpath(__FILE__) == realpath( $_SERVER['SCRIPT_FILENAME'] ) ) {
        header( 'HTTP/1.0 403 Forbidden', TRUE, 403 );
        die( header( 'location: /error.php' ) );
    } //exit if URL accessed directly

    if($_SERVER['HTTP_ORIGIN'] == 'https://users.sussex.ac.uk') { //exit if not from uni server space - probably fairly useless...
        // CHECK HEADERS --------------------------
        header('Access-Control-Allow-Origin: https://users.sussex.ac.uk'); //note these are largely controlled on user side so probably not that useful.
        header('Access-Control-Allow-Methods: PUT');
        header('Access-Control-Allow-Headers: Origin, Content-Type, x-requested-with');
        header('Content-Type: application/json');

        if(!empty($_POST)){exit;}
        if(!empty($_GET)){exit;}
        if(!empty($_FILES)){exit;} //https://st-g.de/2011/04/doing-filename-checks-securely-in-PHP
        

        //CHECK INPUT ARRAY --------------------------
        $input = json_decode(file_get_contents('php://input'), true);
        if (JSON_ERROR_NONE !== json_last_error()){exit;} //https://stackoverflow.com/questions/48242848/how-to-parse-php-json-decode-data-to-jquery-ajax-request    
        // check only allowed variables present
        $allowed_keys = array('metadata', 'trials');
        $result = array_intersect_key($input, array_flip($allowed_keys));
        // Check if the resulting array matches the original input
        if ($input !== $result) {
            //file_put_contents("failure.log", "array contains other keys");
            exit;
        }

        $input = filter_var_array($input,[ //https://stackoverflow.com/questions/37533162/sanitize-json-with-php
            'metadata' => ['filter' => FILTER_SANITIZE_STRING,'flags'=> FILTER_REQUIRE_ARRAY],
            'trials' => ['filter' => FILTER_SANITIZE_INT,'flags'=> FILTER_REQUIRE_ARRAY]
        ]);

        foreach ($input as $key => $value) {
            if ($value === false) {
                //file_put_contents("failure.log", print_r("Validation failed for main input"))// key: $key\n",true));
                exit;
            }
        }


        //CHECK METADATA ARRAY --------------------------
        if (isset($input['metadata']) == true) {//https://www.w3schools.com/php/php_filter.asp
            $metadata = $input['metadata'];

            // Santise
            $metadata_san = array(
                'id' => FILTER_SANITIZE_STRING,
                'userAgent' => FILTER_SANITIZE_STRING,
                'date' => array('filter' => FILTER_SANITIZE_STRING, 'flags' => FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH) // Strip low and high ASCII characters )
            );
            
            // Validate
            // Date time from unity validation
            function validateDateTime($value) {
                $dateFormat = 'Y-m-d H:i:s';
                $date = DateTime::createFromFormat($dateFormat, $value);
                if ($date && $date->format($dateFormat) === $value) {
                    return $value;
                } else {
                    return false;
                }
            };

            //define validation parameters
            $metadata_val = array(
                    'id' => array('filter' => FILTER_VALIDATE_REGEXP, 'options' => array('regexp' => '/^[a-zA-Z0-9]{24}$/')), // Alphanumeric pattern
                    'userAgent' => FILTER_SANITIZE_STRING,
                    'date' => array('filter' => FILTER_CALLBACK, 'options' => 'validateDateTime') // Use the custom validation function
            );

            // run rules
            $san_metadata = array();
            foreach($metadata as &$innerArray) { //note could just stop metadata being a list in the unity project.
                $innerArray = filter_var_array($innerArray,$metadata_san);
                $innerArray = filter_var_array($innerArray,$metadata_val);
                foreach ($innerArray as $key => $value) {
                    if ($value === false) {
                        //file_put_contents("failure.log", print_r("Validation failed for metadata"))// variable: $key\n",true));
                        exit;
                    }
                }
                array_push($san_metadata,$innerArray);
            };

            $id = $san_metadata[0]['id'];
            $input['metadata'] = $san_metadata;
        } else {exit;}


        //CHECK TRIAL DATA ARRAY --------------------------
        if (isset($input['trials']) == true) {//https://www.w3schools.com/php/php_filter.asp
            $trials = $input['trials'];

            // Santisation rules
            $trials_san = array(
                'trial_n' => FILTER_SANITIZE_NUMBER_INT,
                'isi'  => array('filter' => FILTER_SANITIZE_NUMBER_FLOAT, 'flags' => FILTER_FLAG_ALLOW_FRACTION),
                'rt'  => array('filter' => FILTER_SANITIZE_NUMBER_FLOAT, 'flags' => FILTER_FLAG_ALLOW_FRACTION),
                'score' => FILTER_SANITIZE_NUMBER_INT,
                'early_presses' => FILTER_SANITIZE_NUMBER_INT,
            );

            // Validation rules
            $trials_val = array(
                'trial_n' => FILTER_VALIDATE_INT,
                'isi'  => FILTER_VALIDATE_FLOAT,
                'rt'  => FILTER_VALIDATE_FLOAT,
                'score' => FILTER_VALIDATE_INT,
                'early_presses' => FILTER_VALIDATE_INT,
            );

            // run rules
            $san_trials = array();
            foreach($trials as &$innerArray) {
                $innerArray = filter_var_array($innerArray,$trials_san);
                $innerArray = filter_var_array($innerArray,$trials_val);
                foreach ($innerArray as $key => $value) {
                    if ($value === false) {
                        //file_put_contents("failure.log", print_r("Validation failed for trials"))// variable: $key\n",true));
                        exit;
                    }
                }
                array_push($san_trials,$innerArray);
            };
            
            $input['trials'] = $san_trials;
        } else {exit;}

        //its/home/mel29/data/doggo-nogo"
        //public_html/experiments/doggo-nogo
        file_put_contents("../../../data/doggo-nogo/$id.json", json_encode($input)); //https://stackoverflow.com/questions/43519007/usage-of-http-raw-post-data
    } else {exit;}

?>