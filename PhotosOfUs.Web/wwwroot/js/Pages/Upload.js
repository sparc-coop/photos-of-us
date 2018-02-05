$(document).ready(function () {
    $("#drag-and-drop-area").dmUploader({
        url: 'UploadPhoto',
        allowedTypes: 'image/*',
        extFilter: ['jpg', 'jpeg', 'jpe', 'png', 'tiff', 'tif', 'bmp'],
        auto: false,
        queue: false,
        onDragEnter: function () {
            this.addClass('active');
        },
        onDragLeave: function () {
            this.removeClass('active');
        },
        onNewFile: function (id, file) {
            addFile(id, file);
        },
        onBeforeUpload: function (id) {
            updatePhotoStatus(id, 'uploading', 'Uploading...');
            updatePhotoProgress(id, 0, '', true);
        },
        onUploadCanceled: function (id) {
            updatePhotoStatus(id, 'warning', 'Canceled by User');
            updatePhotoProgress(id, 0, 'warning', false);
            updatePhotoControls(id, true, false);
        },
        onUploadSuccess: function (id, data) {
            debugger;
            updatePhotoStatus(id, 'success', 'Upload Complete');
            updatePhotoProgress(id, 100, 'success', false);
        },
        onUploadError: function (id, xhr, status, message) {
            debugger;
            updatePhotoStatus(id, 'danger', message);
            updatePhotoProgress(id, 0, 'danger', false);
        },
        onFileSizeError: function (file) {
            alert("File '" + file.name + "' cannot be added: size excess limit");
        },
        onFileTypeError: function (file) {
            alert("File '" + file.name + "' cannot be added: Accept only file type .jpg, .png, .tif or .bmp");
        },
    });

    $('#btn-save-and-upload').on('click', function (e) {
        e.preventDefault();
        $('#drag-and-drop-area').dmUploader('start');
    });
});

// creates a new file and add it to our list
function addFile(id, file) {
    // animations
    $("#wrapper").css("padding-bottom", "0");

    $("#photo-preview").css("display", "block");
    $("#photo-list").css("display", "block");

    $("#drag-and-drop-area").animate({ height: '80px' }, { duration: 500 });
    $("#drag-and-drop-area span").animate({ 'margin-top': '25px' }, { duration: 500 });
    $('html, body').animate({ scrollTop: parseInt($("#drag-and-drop-area").offset().top - 60) }, 1000)

    $("#photo-preview-name").text(file.name);

    var template = $('#photo-list-template').text();
    template = template.replace('%%filename%%', file.name);
    template = template.replace('%%id-for-photo-code%%', id.toUpperCase());
    template = template.replace('%%id-for-photo-thumbnail%%', id);
    template = template.replace('%%id-for-photo-dimension%%', id);

    template = $(template);
    template.prop('id', 'uploaderFile' + id);
    template.data('file-id', id);

    $('#photos').prepend(template);

    // create thumbnails
    if (typeof FileReader !== 'undefined') {
        var reader = new FileReader();
        var thumbnail = $("#thumbnail-" + id);
        var large_thumbnail = $("#large-photo-thumbnail");

        reader.onload = function (e) {
            large_thumbnail.attr('src', e.target.result);
            thumbnail.attr('src', e.target.result);
        }
    }

    reader.readAsDataURL(file);

    // get dimensions
    var photo = new Image();
    photo.src = window.URL.createObjectURL(file);

    photo.onload = function (e) {
        $("#photo-preview-dimensions").text(photo.naturalWidth + "x" + photo.naturalHeight);
        $(".photo-dimensions-" + id).text(photo.naturalWidth + "x" + photo.naturalHeight);
        window.URL.revokeObjectURL(photo.src);
    };
}

// changes the status messages on our list
function updatePhotoStatus(id, status, message) {
    $('#uploaderFile' + id).find('span').html(message).prop('class', 'status text-' + status);
}

// updates a file progress, depending on the parameters it may animate it or change the color
function updatePhotoProgress(id, percent, color, active) {
    color = (typeof color === 'undefined' ? false : color);
    active = (typeof active === 'undefined' ? true : active);

    var bar = $('#uploaderFile' + id).find('div.progress-bar');

    bar.width(percent + '%').attr('aria-valuenow', percent);
    bar.toggleClass('progress-bar-striped progress-bar-animated', active);

    if (percent === 0) {
        bar.html('');
    } else {
        bar.html(percent + '%');
    }

    if (color !== false) {
        bar.removeClass('bg-success bg-info bg-warning bg-danger');
        bar.addClass('bg-' + color);
    }
}

// toggles the disabled status of star/cancel buttons on one particular file
function updatePhotoControls(id, start, cancel, wasError) {
    wasError = (typeof wasError === 'undefined' ? false : wasError);

    $('#uploaderFile' + id).find('button.start').prop('disabled', !start);
    $('#uploaderFile' + id).find('button.cancel').prop('disabled', !cancel);

    if (!start && !cancel) {
        $('#uploaderFile' + id).find('.controls').fadeOut();
    } else {
        $('#uploaderFile' + id).find('.controls').fadeIn();
    }

    if (wasError) {
        $('#uploaderFile' + id).find('button.start').html('Retry');
    }
}
