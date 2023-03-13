function downloadFile(url, duongDan, nameFile, method) {
    if (method == undefined) {
        method = "POST";
    }
    $.ajax({
        url: url,
        method: method,
        data: {
            //'__RequestVerificationToken': document.getElementsByName("__RequestVerificationToken")[0].value,
            duongDan: duongDan
        },
        dataType: 'binary',
        processData: 'false',
        responseType: 'arraybuffer',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        success: function (data, textStatus, jqXHR) {
            if (textStatus == "success") {
                const type = jqXHR.getResponseHeader('Content-Type');
                var blob = new Blob([data], { type });
                var downloadUrl = URL.createObjectURL(blob);
                var a = document.createElement("a");
                a.href = downloadUrl;
                a.download = nameFile;
                document.body.appendChild(a);
                a.click();
            }
        },
        error: function () {
            console.log("Lỗi tải file");
        }
    });
}
