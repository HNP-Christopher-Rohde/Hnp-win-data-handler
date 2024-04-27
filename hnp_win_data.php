<?php

 /*
  Plugin Name: HNP Win Data Handler
  Description: Handles data via custom REST API for an external C# application with security enhancements.
  Version: 1.0
  Author: HNP C.R.
  Author URI: https://homepage-nach-preis.de/
  License: GPLv3
  License URI: https://www.gnu.org/licenses/gpl-3.0.html
*/

define('HNP_WIN_DATA_SECRET_KEY', '9418BB768671A389');  // Set your secret key here

// Register REST API routes
add_action('rest_api_init', function () {
    register_rest_route('hnp-win-data/v1', '/data', array(
        'methods' => 'GET',
        'callback' => 'hnp_win_data_get_data',
        'permission_callback' => 'hnp_win_data_check_secret_key'
    ));
    register_rest_route('hnp-win-data/v1', '/data', array(
        'methods' => 'POST',
        'callback' => 'hnp_win_data_set_data',
        'permission_callback' => 'hnp_win_data_check_secret_key'
    ));
});

// Function to handle GET request
function hnp_win_data_get_data(WP_REST_Request $request) {
    $value = get_option('hnp_win_data_value', 'No data set.');
    return new WP_REST_Response($value, 200);
}

// Function to handle POST request
function hnp_win_data_set_data(WP_REST_Request $request) {
    $data = $request->get_body();
    $clean_data = sanitize_text_field($data);
    update_option('hnp_win_data_value', $clean_data);
    return new WP_REST_Response('Data updated successfully.', 200);
}

// Permission check to validate secret key
function hnp_win_data_check_secret_key($request) {
    $headers = $request->get_headers();
    $provided_secret = $headers['x_secret_key'][0] ?? '';
    return $provided_secret === HNP_WIN_DATA_SECRET_KEY;
}

// Shortcode to display the data in the frontend
add_shortcode('hnp_win_data_display', 'hnp_win_data_display_shortcode');
function hnp_win_data_display_shortcode() {
    $value = get_option('hnp_win_data_value', 'No data set.');
    return esc_html($value);
}

