window.initSelect2WithData = (dotNetHelper, allTags, selectedTags) => {
    const $select = $('#tagsSelect');

    // Clear any existing options
    $select.empty();

    // Populate with all tags
    allTags.forEach(tag => {
        const option = new Option(tag, tag, false, false);
        $select.append(option);
    });

    // Initialize Select2
    $select.select2({
        tags: true,                  // allow free text
        placeholder: 'Search or add tags',
        allowClear: true
    });

    // Pre-select the selected tags
    $select.val(selectedTags).trigger('change');

    // Handle selection changes
    $select.on('change', function () {
        const values = $(this).val() || [];
        dotNetHelper.invokeMethodAsync('UpdateSelectedTags', values);
    });
};
