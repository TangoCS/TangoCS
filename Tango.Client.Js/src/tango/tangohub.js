import { formatPrefix } from "d3";

window.tangohub = function () {
    const connections = {};
    const url = "tangoHub";
    var instance = {
        init: function (args) {
            if (!connections[url]) {
                var connection = new signalR.HubConnectionBuilder().withUrl("/" + url).build();
                connections[url] = connection;

                connection.on("SetElementValue", function (id, value) {
                    if (args.prefix)
                        id = args.prefix + "_" + id;
                    ajaxUtils.setValue({ id: id, value: value });
                });
                connection.on("ProcessApiResponse", function (apiResponse) {
                    ajaxUtils.processApiResponse(JSON.parse(apiResponse));
                });

                connection.start().then(function () {
                    connection.invoke("SetServiceAction", args.service, args.action, args.key).catch(function (err) {
                        return console.error(err.toString());
                    });
                }).catch(function (err) {
                    return console.error(err.toString());
                });
            }
            else {
                connections[url].invoke("SetServiceAction", args.service, args.action, args.key).catch(function (err) {
                    return console.error(err.toString());
                });
			}
        }
    };
    return instance;
}();