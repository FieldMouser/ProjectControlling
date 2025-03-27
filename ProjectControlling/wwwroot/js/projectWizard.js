function initProjectWizard() {
    let currentStep = 1;
    const totalSteps = 4; // Основные данные, руководитель, исполнители, файлы
    
    function showStep(step) {
        $('.step').hide();
        $(`#step${step}`).show();

        // Управление кнопками
        $('#prevBtn').toggle(step > 1);
        $('#nextBtn').toggle(step < totalSteps);
        $('#submitBtn').toggle(step === totalSteps);
    }
    
    function validateStep(step) {
        let isValid = true;

        if (step === 2 && !$('.select2-leader').val()) {
            alert('Выберите руководителя проекта');
            isValid = false;
        }

        return isValid;
    }
    
    $('#nextBtn').click(function() {
        if (validateStep(currentStep) && currentStep < totalSteps) {
            currentStep++;
            showStep(currentStep);
        }
    });

    $('#prevBtn').click(function() {
        if (currentStep > 1) {
            currentStep--;
            showStep(currentStep);
        }
    });
    
    showStep(1);
}