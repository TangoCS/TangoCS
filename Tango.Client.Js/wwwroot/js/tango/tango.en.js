window.localization = function () {
    var resources = {
        title: {
            systemError: 'System error',
            javascriptError: 'Javascript error',
            noAccess: 'No access',
            pageMissing: 'The page is missing',
            ajaxError: 'Ajax error'
        },
        text: {
            notLoggedSystem: 'You are not logged in to the system',
            insufficientPermissionsOperation: 'Insufficient permissions to perform the operation',
			linkGoToMainPage: '<a href="/">Go to the main page</a>',
			serverUnavailable: 'Server is unavailable or missing<br><br>Address: '
        }
    };

    var instance = {
        resources: resources
    };
    return instance;
}();