$(document).ready(function ()
{
    // Show error_summary if there any validation errors, this is so the summary shows for both front and back end validation.
    if ($(".error-items").length) {
        $(".govuk-error-summary").show();
    }
});