window.initColorPicker = function (el, scope) {
    const pickr = Pickr.create({
        el: el,
        theme: 'nano'
    });
    pickr.on('save', function (color) {
        scope.invokeMethodAsync('SetColor', color.toHEXA().toString());
    });
    return pickr;
};