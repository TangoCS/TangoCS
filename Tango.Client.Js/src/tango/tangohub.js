window.tangohub = function () {
    const connections = {};
    const url = "tangoHub";
    var prefix;
    var instance = {
        init: function (args) {
            prefix = args.prefix;
            if (!connections[url]) {
                var connection = new signalR.HubConnectionBuilder()
                    .withUrl("/" + url)
                    .configureLogging(signalR.LogLevel.Information)
                    .build();
                connections[url] = connection;

                connection.on("SetElementValue", function (id, value) {
                    if (prefix)
                        id = prefix + "_" + id;
                    ajaxUtils.setValue({ id: id, value: value });
                });
                connection.on("ProcessApiResponse", function (apiResponse) {
                    ajaxUtils.processApiResponse(JSON.parse(apiResponse));
                });
                connection.onclose(async () => {
                    await start(connection);
                });

                connection.start().then(function () {
                    console.log("SetServiceAction: " + args.service + "." + args.action + "." + args.key);
                    connection.invoke("SetServiceAction", args.service, args.action, args.key).catch(function (err) {
                        return console.error(err.toString());
                    });
                }).catch(function (err) {
                    return console.error(err.toString());
                });
            }
            else {
                console.log("SetServiceAction: " + args.service + "." + args.action + "." + args.key);
                connections[url].invoke("SetServiceAction", args.service, args.action, args.key).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }
    };

    async function start(connection) {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(function () { start(connection); }, 5000);
        }
    };
    return instance;
}();