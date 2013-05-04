
function showModalWindow() {
	var objBody = document.getElementsByTagName("body").item(0);

	var objOverlay = document.getElementById('modalWindowBack');
	if (objOverlay == null) {
		objOverlay = document.createElement("div");
		objOverlay.setAttribute('id', 'modalWindowBack');
		objOverlay.style.display = 'none';
		objBody.insertBefore(objOverlay, objBody.firstChild);
		
		var objLoadingImage = document.createElement("img");
		objLoadingImage.src = '/i/n/loading.gif';
		objLoadingImage.setAttribute('id', 'loadingImage');
		objLoadingImage.setAttribute('alt', 'Идет обращение к серверу. Пожалуйста, подождите...');
		objLoadingImage.style.position = 'absolute';
		objLoadingImage.style.zIndex = '150';
		objOverlay.appendChild(objLoadingImage);
	}
	alert('urra!');
}