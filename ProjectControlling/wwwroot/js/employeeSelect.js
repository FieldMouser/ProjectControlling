function initLeaderSelect() {
    $('.select2-leader').select2({
        ajax: {
            url: '/Projects/SearchEmployees',
            dataType: 'json',
            delay: 250,
            data: function(params) {
                return { term: params.term };
            },
            processResults: function(data) {
                return { results: data };
            }
        },
        minimumInputLength: 2,
        placeholder: 'Выберите руководителя',
        templateResult: formatEmployee,
        templateSelection: formatEmployeeSelection
    });
}

function initWorkersSelect() {
    $('.select2-workers').select2({
        ajax: {
            url: '/Projects/SearchEmployees',
            dataType: 'json',
            delay: 250,
            data: function(params) {
                return { term: params.term };
            },
            processResults: function(data) {
                return { results: data };
            }
        },
        minimumInputLength: 2,
        placeholder: 'Выберите исполнителей',
        multiple: true,
        closeOnSelect: false,
        templateResult: formatEmployee,
        templateSelection: formatEmployeeSelection
    });
}

function formatEmployee(employee) {
    if (!employee.id) return employee.text;
    return $('<span>' + employee.text + '</span>');
}

function formatEmployeeSelection(employee) {
    return employee.text || employee.email;
}

$(document).ready(function() {
    initLeaderSelect();
    initWorkersSelect();
});