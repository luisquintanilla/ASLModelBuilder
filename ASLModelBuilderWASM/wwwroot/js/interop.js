// Start getting data
window.Initialize = async (elements) => {

    function handleSuccess(stream) {
        window.stream = stream;
        document.getElementById(elements['video']).srcObject = stream;
    }

    try {
        let stream = await navigator.mediaDevices.getUserMedia({ audio: false, video: { width: 400, height: 240 } });
        handleSuccess(stream);
    } catch (e) {
        console.log(e);
    }
}

//DrawImage
window.Snap = async (src, dest) => {
    let video = document.getElementById(src);
    let ctx = get2DContext(dest);
    ctx.drawImage(video, 0, 0, 400, 240);
}

// Get image as base64 string
window.GetImageData = async (el, format) => {
    let canvas = document.getElementById(el);
    let dataUrl = canvas.toDataURL(format)
    return dataUrl.split(',')[1];
}

// Helper functions
function get2DContext(el) {
    return document.getElementById(el).getContext('2d');
}