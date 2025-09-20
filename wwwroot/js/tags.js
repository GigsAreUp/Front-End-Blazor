window.initSelect2WithDataTags = (dotNetRef, data, selectedValues) => {
    // `data` should be an array of strings: ["Tag1", "Tag2", ...]
    // `selectedValues` should also be an array of strings
    const $select = $('#tagsSelect3');

    // Destroy old instance if exists
    if ($select.hasClass("select2-hidden-accessible")) {
        $select.select2('destroy');
    }

    $select.empty(); // clear old options

    // Populate options
    data.forEach(tag => {
        const option = new Option(tag, tag, false, selectedValues.includes(tag));
        $select.append(option);
    });

    // Initialize Select2 (NO free typing, only dropdown)
    $select.select2({
        tags: false,               // 🚫 disables new tag creation
        multiple: true,
        width: '100%',
        placeholder: "Select tags"
    });

    // Handle change -> call Blazor back
    $select.on('change', function () {
        const selected = $(this).val() || [];
        dotNetRef.invokeMethodAsync("OnTagsChanged", selected);
    });
};
