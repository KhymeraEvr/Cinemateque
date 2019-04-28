function getActors() {
    var uri = "actors"
    $.getJSON(uri)
        .done(function (data) {
            getTable("actors", data)
        });       
}

function getDirectors() {
    var uri = "directors"
    $.getJSON(uri)
        .done(function (data) {
            getTable("directors", data)
        });
}

function getFilms() {
    var uri = "films"
    $.getJSON(uri)
        .done(function (data) {
            getTable("films", data)
        });
}

function getTable(selector, data) {
    var table = document.getElementById(selector)
    var tr = table.insertRow(-1);    

    var col = [];
    for (var i = 0; i < data.length; i++) {
        for (var key in data[i]) {
            if (col.indexOf(key) === -1) {
                col.push(key);
            }
        }
    }

    for (var i = 0; i < col.length; i++) {
        var th = document.createElement("th");    
        th.innerHTML = col[i];
        tr.appendChild(th);
    }

    for (var i = 0; i < data.length; i++) {

        tr = table.insertRow(-1);

        for (var j = 0; j < col.length; j++) {
            var tabCell = tr.insertCell(-1);
            tabCell.innerHTML = data[i][col[j]];
        }
    }
}
