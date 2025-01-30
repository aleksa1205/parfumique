export default function Output({outputValue, active}: {
    outputValue: string,
    active: boolean
}) {
    
    if (!active) return <></>

    return (
        <div className="space-y-4 mt-6">
            {outputValue !== '' && (
                <>
                    <label htmlFor="json-output" className="block text-lg font-bold text-gray-700">
                        Output
                    </label>
                    <textarea
                        id="json-output"
                        value={outputValue}
                        disabled={true}
                        rows={30}
                        placeholder='e.g., { "key": "value" }'
                        className="w-full p-3 text-sm/4 text-neutral-100 bg-neutral-700 border-2 border-neutral-300 rounded-lg focus:outline-none focus:border-brand-500 hover:border-brand-700 resize-none"
                    ></textarea>
                </>
            )}
      </div>
    )
}