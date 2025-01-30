export default function Description({description} : {description: string}) {
    return (
        <p className="text-neutral-400 mb-4 font-light">
            {description}
        </p>
    )
}