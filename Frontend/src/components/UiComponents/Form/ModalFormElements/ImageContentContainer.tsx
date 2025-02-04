export default function ImageContentContainer({children}: {
    children: any;
}) {
    return (
        <div
            className="flex flex-col md:grid grid-cols-2 gap-x-[3rem] justify-center items-center mt-4 mb-8
                       [&>p]: text-center"
        >
            {children}
        </div>
    )
}